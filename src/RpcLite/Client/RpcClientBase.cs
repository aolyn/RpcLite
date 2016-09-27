//#define LogDuration
#if DEBUG && LogDuration
using System.Diagnostics;
#endif

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using RpcLite.Formatters;
using RpcLite.Logging;

namespace RpcLite.Client
{
	/// <summary>
	/// 
	/// </summary>
	/// <typeparam name="TContract">contract interface</typeparam>
	public class RpcClientBase<TContract> : IRpcClient<TContract>
		where TContract : class
	{
		/// <summary>
		/// base url of service
		/// </summary>
		public string Address
		{
			get { return Cluster?.Address; }
			set
			{
				if (Cluster != null)
					Cluster.Address = value;
			}
		}

		//private IFormatter _formatter = new JsonFormatter();
		/// <summary>
		/// Formatter
		/// </summary>
		public IFormatter Formatter { get; set; }

		///// <summary>
		///// Channel to transport data with service
		///// </summary>
		//public IClientChannel Channel { get; set; }

		/// <summary>
		/// Channel to transport data with service
		/// </summary>
		public ICluster<TContract> Cluster { get; set; }

		/// <summary>
		/// 
		/// </summary>
		/// <param name="action"></param>
		/// <param name="request"></param>
		/// <param name="argumentType"></param>
		/// <param name="returnType"></param>
		/// <returns></returns>
		protected object GetResponse<TResult>(string action, object request, Type argumentType, Type returnType)
		{
#if DEBUG && LogDuration
			var stopwatch = Stopwatch.StartNew();
#endif
			var response = GetResponseAsync<TResult>(action, request, argumentType, returnType);
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
		/// <param name="argumentType"></param>
		/// <param name="returnType"></param>
		/// <returns></returns>
		protected Task<TResult> GetResponseAsync<TResult>(string action, object request, Type argumentType, Type returnType)
		{
			if (Formatter == null)
				throw new ServiceException("Formatter can't be null");

			var resultObj = DoRequestAsync<TResult>(action, request, argumentType, returnType);
			return resultObj;
		}

		private Task<TResult> DoRequestAsync<TResult>(string action, object param, Type argumentType, Type returnType)
		{
			var sendTask = SendAsync(action, param, argumentType);

#if DEBUG && LogDuration
			var duration0 = stopwatch1.GetAndRest();
#endif

			var task = sendTask.ContinueWith(tsk =>
			{
#if DEBUG && LogDuration
				var duration1 = stopwatch1.GetAndRest();
#endif
				if (tsk.Exception != null)
					throw tsk.Exception.InnerException;

				var resultMessage = tsk.Result;
				if (resultMessage == null)
					throw new ClientException("get service data error");

				return GetResult<TResult>(tsk.Result, returnType);
			});

			return task;
		}

		private Task<ResponseMessage> SendAsync(string action, object param, Type argumentType)
		{
			var mime = Formatter.SupportMimes.First();
			var headDic = new Dictionary<string, string>
			{
				{"Content-Type",mime},
				{"Accept",mime},
			};

			var content = new MemoryStream();
			if (argumentType != null)
			{
				try
				{
					Formatter.Serialize(content, param, argumentType);
				}
				catch (Exception ex)
				{
					throw new SerializeRequestException("Serialize Request Object failed.", ex);
				}
			}
			content.Position = 0;

#if DEBUG && LogDuration
			var stopwatch1 = Stopwatch.StartNew();
#endif

			var sendTask = Cluster.SendAsync(action, content, headDic);
			return sendTask;
		}

		private TResult GetResult<TResult>(ResponseMessage resultMessage, Type returnType)
		{
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

			if (resultMessage.Headers.Count == 0)
				throw new ServiceException("service url is not a service address");

			var exceptionAssembly = resultMessage.Headers[HeaderName.ExceptionAssembly];
			var exceptionTypeName = resultMessage.Headers[HeaderName.ExceptionType];

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

				exObj = Formatter.SupportException
					? Formatter.Deserialize(resultMessage.Result, exceptionType)
					: Activator.CreateInstance(exceptionType);
			}
			catch (Exception ex)
			{
				throw new ClientException("Deserialize Response failed", ex);
			}

			if (exObj != null)
				throw (Exception)exObj;

			//return default(TResult);
			throw new ServiceException("exception occored but no exception data transported");
		}

		/// <summary>
		/// 
		/// </summary>
		//public TContract Client => this as TContract;

		/// <summary>
		/// 
		/// </summary>
		internal AppHost AppHost { get; set; }
	}

	//internal static class StaticDataHolder
	//{
	//	internal static readonly ConcurrentDictionary<string, DateTime> DotFoundAssemblyDictionary = new ConcurrentDictionary<string, DateTime>();
	//}
}
