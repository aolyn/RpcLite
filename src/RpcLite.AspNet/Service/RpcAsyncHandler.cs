using System;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using RpcLite.Logging;

namespace RpcLite.Service
{
	/// <summary>
	/// <para>RpcLite AsyncHandler to process service request</para>
	/// <para>in mono 3.12 RpcAsyncHandler can't work use Sync Handler</para>
	/// </summary>
	public class RpcAsyncHandler : IHttpAsyncHandler
	{
		#region Sync

		/// <summary>
		/// 
		/// </summary>
		/// <param name="context"></param>
		public void ProcessRequest(HttpContext context)
		{
			ProcessRequestInternal(context);
		}

		internal static void ProcessRequestInternal(HttpContext context)
		{
			using (var are = new AutoResetEvent(false))
			{
				// ReSharper disable once AccessToDisposedClosure
				var ar = BeginProcessRequestInternal(context, r => are.Set(), null);

				if (ar != null)
				{
					if (ar.CompletedSynchronously)
					{
						EndProcessRequestInternal(ar);
					}
					else
					{
						are.WaitOne();
						EndProcessRequestInternal(ar);
					}
				}
			}
		}

		/// <summary>
		/// 
		/// </summary>
		public bool IsReusable
		{
			get
			{
				return true;
			}
		}

		#endregion

		/// <summary>
		/// 
		/// </summary>
		/// <param name="context"></param>
		/// <param name="cb"></param>
		/// <param name="extraData"></param>
		/// <returns></returns>
		public IAsyncResult BeginProcessRequest(HttpContext context, AsyncCallback cb, object extraData)
		{
			return BeginProcessRequestInternal(context, cb, extraData);
		}

		private static IAsyncResult BeginProcessRequestInternal(HttpContext context, AsyncCallback cb, object extraData)
		{
			var request = context.Request;
			var response = context.Response;

			var requestPath = request.Path;
			// ReSharper disable once PossibleNullReferenceException
			requestPath = request.ApplicationPath.Length == 1
				? "~" + requestPath
				: "~" + requestPath.Substring(request.ApplicationPath.Length);

			var serviceRequest = new ServiceRequest
			{
				RequestStream = request.InputStream,
				Path = requestPath,
				ContentType = request.ContentType,
				ContentLength = request.ContentLength,
			};

			var serviceResponse = new ServiceResponse
			{
				ResponseStream = response.OutputStream,
			};

			var serviceContext = new ServiceContext
			{
				Request = serviceRequest,
				Response = serviceResponse,
				ExtraData = extraData,
				ExecutingContext = (object)context,
			};

			LogHelper.Debug("BeginProcessRequest: " + request.Url);

			try
			{
				var result = RpcProcessor.ProcessAsync(serviceContext);
				var ar = ToBegin(result, cb, serviceContext);
				return ar;
			}
			catch (Exception ex)
			{
				serviceContext.Exception = ex;
				return new ServiceAsyncResult
				{
					AsyncState = serviceContext,
					IsCompleted = true,
					CompletedSynchronously = true,
					AsyncWaitHandle = null,
				};
			}
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="task"></param>
		/// <param name="callback"></param>
		/// <param name="state"></param>
		/// <returns></returns>
		public static IAsyncResult ToBegin(Task task, AsyncCallback callback, object state)
		{
			if (task == null)
				throw new ArgumentNullException(nameof(task));

			var tcs = new TaskCompletionSource<object>(state);
			task.ContinueWith(t =>
			{
				if (task.IsFaulted)
				{
					if (task.Exception != null)
						tcs.TrySetException(task.Exception.InnerExceptions);
				}
				else if (task.IsCanceled)
				{
					tcs.TrySetCanceled();
				}
				else
				{
					tcs.TrySetResult(null);
					//tcs.TrySetResult(RpcAction.GetTaskResult(t));
				}

				callback?.Invoke(tcs.Task);
			}/*, TaskScheduler.Default*/);

			return tcs.Task;
		}


		/// <summary>
		/// 
		/// </summary>
		/// <param name="result"></param>
		public void EndProcessRequest(IAsyncResult result)
		{
			EndProcessRequestInternal(result);
		}

		private static void EndProcessRequestInternal(IAsyncResult result)
		{
			LogHelper.Debug("RpcAsyncHandler.EndProcessRequest start RpcService.EndProcessRequest(result);");

			var context = result.AsyncState as ServiceContext;
			if (context == null)
			{
				LogHelper.Error("ServiceContext is null", null);
				return;
			}

			var httpContext = (HttpContext)context.ExecutingContext;
			httpContext.Response.ContentType = context.Response.ContentType;
			if (context.Exception != null)
			{
				httpContext.Response.Headers["RpcLite-ExceptionType"] = context.Exception.GetType().FullName;
				httpContext.Response.Headers["RpcLite-ExceptionAssembly"] = context.Exception.GetType().GetTypeInfo().Assembly.FullName;
				httpContext.Response.Headers["RpcLite-StatusCode"] = ((int)HttpStatusCode.InternalServerError).ToString();
				httpContext.Response.StatusCode = (int)HttpStatusCode.InternalServerError;

				if (context.Formatter == null)
					throw context.Exception;

				context.Formatter.Serialize(context.Response.ResponseStream, context.Exception);
			}
			else
			{
				httpContext.Response.Headers["RpcLite-StatusCode"] = ((int)HttpStatusCode.OK).ToString();

				if (context.Result != null && context.Action.HasReturnValue)
					context.Formatter.Serialize(context.Response.ResponseStream, context.Result);
			}

			LogHelper.Debug("RpcAsyncHandler.EndProcessRequest end RpcService.EndProcessRequest(result);");
		}
	}
}