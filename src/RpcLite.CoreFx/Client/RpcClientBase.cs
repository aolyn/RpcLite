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
using RpcLite.Net;

namespace RpcLite.Client
{
	/// <summary>
	/// 
	/// </summary>
	/// <typeparam name="TContract">contract interface</typeparam>
	public class RpcClientBase<TContract> where TContract : class
	{
		/// <summary>
		/// base url of service
		/// </summary>
		public string BaseUrl { get; set; }

		private IFormatter _formatter = new JsonFormatter();
		/// <summary>
		/// Formatter
		/// </summary>
		public IFormatter Formatter { get { return _formatter; } set { _formatter = value; } }

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

		///// <summary>
		///// 
		///// </summary>
		///// <param name="action"></param>
		///// <param name="request"></param>
		///// <param name="returnType"></param>
		///// <returns></returns>
		//protected object GetResponse(string action, object request, Type returnType)
		//{
		//	var response = DoRequest(action, request, returnType);

		//	return response;
		//}

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

			var url = BaseUrl + action;
			var resultMessageTask = WebRequestHelper.PostAsync(url, content, headDic);

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
						var resultObj = Formatter.Deserilize(resultMessage.Result, objType);
						return (TResult)resultObj;
					}
					//catch (Exception ex)
					//{
					//	return default(TResult);
					//	//throw;
					//}
					finally
					{
						resultMessage.Dispose();
					}
				}

				var exceptionAssembly = resultMessage.Header["RpcLite-ExceptionAssembly"];
				var exceptionType = resultMessage.Header["RpcLite-ExceptionType"];

				if (string.IsNullOrWhiteSpace(exceptionAssembly) || string.IsNullOrWhiteSpace(exceptionType))
				{
					throw new ClientException("exception occored, but no ExceptionAssembly and ExceptionType returned");
				}

#if NETCORE
				var asm = Assembly.Load(new AssemblyName(exceptionAssembly));
#else
				var asm = Assembly.Load(exceptionAssembly);
#endif
				var exType = asm.GetType(exceptionType);

				var exObj = Formatter.Deserilize(resultMessage.Result, exType);
				if (exObj != null)
					throw (Exception)exObj;

				return default(TResult);
			});

			return task;
		}

		// ReSharper disable once UnusedMember.Local
		private Task<TResult> DoRequestAsync2<TResult>(string action, object param, Type returnType, string mime)
		{
			var headDic = new Dictionary<string, string>
			{
				{"Content-Type",mime},
				{"Accept",mime},
			};

			//var json = JsonConvert.SerializeObject(param);

			var content = new FormaterContent(Formatter, param);

#if DEBUG && LogDuration
			var stopwatch1 = Stopwatch.StartNew();
#endif

			var url = BaseUrl + action;
			var resultMessageTask = WebRequestHelper.PostAsync(url, content, headDic);

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
						var resultObj = Formatter.Deserilize(resultMessage.Result, objType);
						return (TResult)resultObj;
					}
					//catch (Exception ex)
					//{
					//	return default(TResult);
					//	//throw;
					//}
					finally
					{
						resultMessage.Dispose();
					}
				}

				var exceptionAssembly = resultMessage.Header["RpcLite-ExceptionAssembly"];
				var exceptionType = resultMessage.Header["RpcLite-ExceptionType"];

				if (string.IsNullOrWhiteSpace(exceptionAssembly) || string.IsNullOrWhiteSpace(exceptionType))
				{
					throw new ClientException("exception occored, but no ExceptionAssembly and ExceptionType returned");
				}

#if NETCORE
				var asm = Assembly.Load(new AssemblyName(exceptionAssembly));
#else
				var asm = Assembly.Load(exceptionAssembly);
#endif
				var exType = asm.GetType(exceptionType);

				var exObj = Formatter.Deserilize(resultMessage.Result, exType);
				if (exObj != null)
					throw (Exception)exObj;

				return default(TResult);
			});

			return task;
		}

		/// <summary>
		/// 
		/// </summary>
		public TContract Client
		{
			get { return this as TContract; }
		}

		//private object DoRequest(string action, object param, Type returnType)
		//{
		//	if (_formatter == null)
		//		throw new ServiceException("Formatter can't be null");

		//	var mime = _formatter.SupportMimes.First();

		//	var resultObj = DoRequest(action, param, returnType, mime);
		//	return resultObj;

		//	//var resultObj = DoRequestAsync(action, param, returnType, mime);
		//	//return resultObj.Result;

		//}

		//		private object DoRequest(string action, object param, Type returnType, string mime)
		//		{
		//			var headDic = new Dictionary<string, string>
		//			{
		//				{"Content-Type",mime},
		//				{"Accept",mime},
		//			};

		//			var json = JsonConvert.SerializeObject(param);

		//			var url = BaseUrl + action;
		//			var resultMessage = WebRequestHelper.Post(url, json, Encoding.UTF8, headDic);
		//			if (resultMessage == null)
		//				throw new ClientException("get service data error");

		//			if (resultMessage.IsSuccess)
		//			{
		//				if (string.IsNullOrEmpty(resultMessage.Result) || returnType == null)
		//					return null;

		//#if NETCORE
		//				var objType = returnType.GetTypeInfo().BaseType == typeof(Task)
		//					? returnType.GetGenericArguments()[0]
		//					: returnType;
		//#else
		//				var objType = returnType.BaseType == typeof(Task)
		//					? returnType.GetGenericArguments()[0]
		//					: returnType;
		//#endif

		//				var resultObj = JsonConvert.DeserializeObject(resultMessage.Result, objType);
		//				return resultObj;
		//			}

		//			Type exceptionType;
		//			var assemblyName = resultMessage.Header["RpcLite-ExceptionAssembly"];
		//			if (string.IsNullOrWhiteSpace(assemblyName) || StaticDataHolder.DotFoundAssemblyDictionary.ContainsKey(assemblyName))
		//			{
		//				exceptionType = typeof(Exception);
		//			}
		//			else
		//			{
		//				try
		//				{
		//#if NETCORE
		//					var asm = Assembly.Load(new AssemblyName(assemblyName));
		//#else
		//					var asm = Assembly.Load(assemblyName);
		//#endif
		//					exceptionType = asm.GetType(resultMessage.Header["RpcLite-ExceptionType"]);
		//				}
		//				catch (FileNotFoundException ex)
		//				{
		//					LogHelper.Error("load exception assebmly error, exception assmbly not found", ex);
		//					exceptionType = typeof(Exception);
		//					StaticDataHolder.DotFoundAssemblyDictionary.TryAdd(assemblyName, DateTime.Now);
		//				}
		//			}

		//			var exceptionObject = JsonConvert.DeserializeObject(resultMessage.Result, exceptionType);
		//			if (exceptionObject != null)
		//				throw (Exception)exceptionObject;

		//			return null;
		//		}

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
			var uri = ClientAddressResolver<TContract>.GetAddress();
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
			client.BaseUrl = baseUrl;
			return client;
		}

	}

	internal static class StaticDataHolder
	{
		internal static readonly ConcurrentDictionary<string, DateTime> DotFoundAssemblyDictionary = new ConcurrentDictionary<string, DateTime>();
	}
}
