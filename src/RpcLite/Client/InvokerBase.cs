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
	/// <typeparam name="TContract"></typeparam>
	public abstract class InvokerBase<TContract> : IInvoker<TContract>
	{
		/// <summary>
		/// 
		/// </summary>
		public virtual string Address { get; set; }

		/// <summary>
		/// service name
		/// </summary>
		public string Name { get;private set; }

		/// <summary>
		/// service group
		/// </summary>
		public string Group { get; private set; }

		/// <summary>
		/// 
		/// </summary>
		/// <typeparam name="TResult"></typeparam>
		/// <param name="action"></param>
		/// <param name="param"></param>
		/// <param name="actionInfo"></param>
		/// <param name="formatter"></param>
		/// <returns></returns>
		public virtual Task<TResult> InvokeAsync<TResult>(string action, object param, RpcActionInfo actionInfo, IFormatter formatter)
		{
			var argumentType = actionInfo.ArgumentType;
			var returnType = actionInfo.ResultType;

			var sendTask = SendAsync(action, param, argumentType, formatter);

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

				return GetResult<TResult>(resultMessage, returnType, formatter);
			});

			return task;
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="action"></param>
		/// <param name="param"></param>
		/// <param name="argumentType"></param>
		/// <param name="formatter"></param>
		/// <returns></returns>
		protected virtual Task<IResponseMessage> SendAsync(string action, object param, Type argumentType, IFormatter formatter)
		{
			var mime = formatter.SupportMimes.First();
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
					formatter.Serialize(content, param, argumentType);
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

			var sendTask = SendAsync(action, content, headDic);
			return sendTask;
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="action"></param>
		/// <param name="content"></param>
		/// <param name="headers"></param>
		/// <returns></returns>
		protected abstract Task<IResponseMessage> SendAsync(string action, Stream content, IDictionary<string, string> headers);

		/// <summary>
		/// 
		/// </summary>
		/// <typeparam name="TResult"></typeparam>
		/// <param name="resultMessage"></param>
		/// <param name="returnType"></param>
		/// <param name="formatter"></param>
		/// <returns></returns>
		protected virtual TResult GetResult<TResult>(IResponseMessage resultMessage, Type returnType, IFormatter formatter)
		{
			if (resultMessage.IsSuccess)
			{
				if (resultMessage.Result == null || returnType == null)
					return default(TResult);

				var objType = typeof(TResult);

				try
				{
					var resultObj = formatter.Deserialize(resultMessage.Result, objType);
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

				exObj = formatter.SupportException
					? formatter.Deserialize(resultMessage.Result, exceptionType)
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
		/// optional initialize method
		/// </summary>
		/// <param name="name">service name</param>
		/// <param name="group">service group</param>
		public void Initialize(string name, string group)
		{
			Name = name;
			Group = group;
		}
	}
}
