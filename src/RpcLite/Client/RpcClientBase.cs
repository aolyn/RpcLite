//#define LogDuration
#if DEBUG && LogDuration
using System.Diagnostics;
#endif

using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using RpcLite.Formatters;
using RpcLite.Logging;
using RpcLite.Service;

namespace RpcLite.Client
{
	/// <summary>
	/// 
	/// </summary>
	/// <typeparam name="TContract">contract interface</typeparam>
	public class RpcClientBase<TContract> : IRpcClient
		where TContract : class
	{
		/// <summary>
		/// base url of service
		/// </summary>
		public string Address
		{
			get { return Channel?.Address; }
			set
			{
				if (Channel != null)
					Channel.Address = value;
			}
		}

		private IFormatter _formatter = new JsonFormatter();
		/// <summary>
		/// Formatter
		/// </summary>
		public IFormatter Formatter { get { return _formatter; } set { _formatter = value; } }

		/// <summary>
		/// Channel to transport data with service
		/// </summary>
		public IClientChannel Channel { get; set; }

		/// <summary>
		/// 
		/// </summary>
		/// <param name="action"></param>
		/// <param name="request"></param>
		/// <param name="returnType"></param>
		/// <returns></returns>
		protected object GetResponse<TResult>(string action, object request, Type returnType)
		{
#if DEBUG && LogDuration
			var stopwatch = Stopwatch.StartNew();
#endif
			var response = GetResponseAsync<TResult>(action, request, returnType);
			try
			{
#if DEBUG && LogDuration
				stopwatch.Stop();
				var duration = stopwatch.ElapsedMilliseconds;
#endif
				var result = response.Result;
				return result;
			}
			catch (AggregateException ex)
			{
				throw ex.InnerException;
			}
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="action"></param>
		/// <param name="request"></param>
		/// <param name="returnType"></param>
		/// <returns></returns>
		protected Task<TResult> GetResponseAsync<TResult>(string action, object request, Type returnType)
		{
			if (_formatter == null)
				throw new ServiceException("Formatter can't be null");

			var mime = _formatter.SupportMimes.First();

			var resultObj = DoRequestAsync<TResult>(action, request, returnType, mime);
			return resultObj;
		}

		private Task<TResult> DoRequestAsync<TResult>(string action, object param, Type returnType, string mime)
		{
			var headDic = new Dictionary<string, string>
			{
				{"Content-Type",mime},
				{"Accept",mime},
			};

			//var content = new FormaterContent(Formatter, param);

			var content = new MemoryStream();
			Formatter.Serialize(content, param);
			content.Position = 0;

#if DEBUG && LogDuration
			var stopwatch1 = Stopwatch.StartNew();
#endif

			var resultMessageTask = Channel.SendAsync(action, content, headDic);

#if DEBUG && LogDuration
			var duration0 = stopwatch1.GetAndRest();
#endif

			var task = resultMessageTask.ContinueWith(tsk =>
			{
#if DEBUG && LogDuration
				var duration1 = stopwatch1.GetAndRest();
#endif
				if (tsk.Exception != null)
					throw tsk.Exception.InnerException;

				var resultMessage = tsk.Result;
				if (resultMessage == null)
					throw new ClientException("get service data error");

				if (resultMessage.IsSuccess)
				{
					if (resultMessage.Result == null || returnType == null)
						return default(TResult);

					var objType = typeof(TResult);

					try
					{
						var resultObj = Formatter.Deserialize(resultMessage.Result, objType);
						return (TResult)resultObj;
					}
					catch (Exception ex)
					{
						//return default(TResult);
						//throw;
						throw new ServiceException("parse data received error", ex);
					}
					finally
					{
						resultMessage.Dispose();
					}
				}

				if (resultMessage.Header.Count == 0)
					throw new ServiceException("service url is not a service address");

				var exceptionAssembly = resultMessage.Header["RpcLite-ExceptionAssembly"];
				var exceptionTypeName = resultMessage.Header["RpcLite-ExceptionType"];

				if (string.IsNullOrWhiteSpace(exceptionAssembly) || string.IsNullOrWhiteSpace(exceptionTypeName))
				{
					throw new ClientException("exception occored, but no ExceptionAssembly and ExceptionType returned");
				}

				Type exceptionType;
				try
				{
#if NETCORE
					var asm = Assembly.Load(new AssemblyName(exceptionAssembly));
#else
					var asm = Assembly.Load(exceptionAssembly);
#endif
					exceptionType = asm.GetType(exceptionTypeName);
				}
				catch (Exception ex)
				{
					LogHelper.Error("can't find exception type " + exceptionTypeName, ex);
					exceptionType = typeof(Exception);
				}

				object exObj;
				try
				{
					//var buf = new byte[8192];
					//var readLength = resultMessage.Result.Read(buf, 0, buf.Length);
					//var json = Encoding.UTF8.GetString(buf);

					exObj = Formatter.Deserialize(resultMessage.Result, exceptionType);
				}
				catch (Exception ex)
				{
					throw new ClientException("Deserialize Response failed", ex);
				}

				if (exObj != null)
					throw (Exception)exObj;

				//return default(TResult);
				throw new ServiceException("exception occored but no exception data transported");
			});

			return task;
		}

		// ReSharper disable once UnusedMember.Local
//		private Task<TResult> DoRequestAsync2<TResult>(string action, object param, Type returnType, string mime)
//		{
//			var headDic = new Dictionary<string, string>
//			{
//				{"Content-Type",mime},
//				{"Accept",mime},
//			};

//			//var json = JsonConvert.SerializeObject(param);

//			var content = new FormaterContent(Formatter, param);

//#if DEBUG && LogDuration
//			var stopwatch1 = Stopwatch.StartNew();
//#endif

//			var url = BaseUrl + action;
//			var resultMessageTask = WebRequestHelper.PostAsync(url, content, headDic);

//#if DEBUG && LogDuration
//			var duration0 = stopwatch1.GetAndRest();
//#endif

//			var task = resultMessageTask.ContinueWith(tsk =>
//			{
//#if DEBUG && LogDuration
//				var duration1 = stopwatch1.GetAndRest();
//#endif
//				if (tsk.Exception != null)
//					throw tsk.Exception.InnerException;

//				var resultMessage = tsk.Result;
//				if (resultMessage == null)
//					throw new ClientException("get service data error");

//				if (resultMessage.IsSuccess)
//				{
//					if (resultMessage.Result == null || returnType == null)
//						return default(TResult);

//					var objType = typeof(TResult);

//					try
//					{
//						var resultObj = Formatter.Deserialize(resultMessage.Result, objType);
//						return (TResult)resultObj;
//					}
//					//catch (Exception ex)
//					//{
//					//	return default(TResult);
//					//	//throw;
//					//}
//					finally
//					{
//						resultMessage.Dispose();
//					}
//				}

//				var exceptionAssembly = resultMessage.Header["RpcLite-ExceptionAssembly"];
//				var exceptionType = resultMessage.Header["RpcLite-ExceptionType"];

//				if (string.IsNullOrWhiteSpace(exceptionAssembly) || string.IsNullOrWhiteSpace(exceptionType))
//				{
//					throw new ClientException("exception occored, but no ExceptionAssembly and ExceptionType returned");
//				}

//#if NETCORE
//				var asm = Assembly.Load(new AssemblyName(exceptionAssembly));
//#else
//				var asm = Assembly.Load(exceptionAssembly);
//#endif
//				var exType = asm.GetType(exceptionType);

//				var exObj = Formatter.Deserialize(resultMessage.Result, exType);
//				if (exObj != null)
//					throw (Exception)exObj;

//				return default(TResult);
//			});

//			return task;
//		}

		/// <summary>
		/// 
		/// </summary>
		public TContract Client
		{
			get { return this as TContract; }
		}


		private static Lazy<Func<RpcClientBase<TContract>>> _func = new Lazy<Func<RpcClientBase<TContract>>>(() =>
		{
			var type = ClientWrapper.WrapInterface<TContract>();
			var func = TypeCreator.GetCreateInstanceFunc(type) as Func<RpcClientBase<TContract>>;
			return func;
		}, true);

		/// <summary>
		/// 
		/// </summary>
		/// <returns></returns>
		public static RpcClientBase<TContract> GetInstance()
		{
			return GetInstance(GetDefaultBaseUrl());
		}

		private static string GetDefaultBaseUrl()
		{
			//var uri = ClientAddressResolver<TContract>.GetAddress();
			var uri = RpcProcessor.ServiceHost.RegistryManager.GetAddress<TContract>();
			return uri == null ? null : uri.ToString();
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="baseUrl"></param>
		/// <returns></returns>
		public static RpcClientBase<TContract> GetInstance(string baseUrl)
		{
			if (_func.Value == null)
				throw new ClientException("GetCreateInstanceFunc Error.");

			var client = _func.Value();
			client.Address = baseUrl;
			client.Channel = new HttpClientChannel(baseUrl);
			return client;
		}

	}

	internal static class StaticDataHolder
	{
		internal static readonly ConcurrentDictionary<string, DateTime> DotFoundAssemblyDictionary = new ConcurrentDictionary<string, DateTime>();
	}
}
