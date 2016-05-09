using System;
using System.Net;
using System.Threading;
using System.Web;
using RpcLite.Config;
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
				ExecutingContext = context,
			};

			LogHelper.Debug("BeginProcessRequest: " + request.Url);

			try
			{
				return Process(serviceContext, cb);
			}
			//catch (ThreadAbortException ex)
			//{
			//	serviceContext.Exception = ex;
			//	return new ServiceAsyncResult
			//	{
			//		AsyncState = context,
			//		IsCompleted = true,
			//		CompletedSynchronously = true,
			//		AsyncWaitHandle = null,
			//	};
			//}
			//catch (TypeInitializationException ex)
			//{
			//	serviceContext.Exception = ex;
			//	return new ServiceAsyncResult
			//	{
			//		AsyncState = serviceContext,
			//		IsCompleted = true,
			//		CompletedSynchronously = true,
			//		AsyncWaitHandle = null,
			//	};
			//}
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

		private static IAsyncResult Process(ServiceContext serviceContext, AsyncCallback cb)
		{
			//get formatter from content type
			var formatter = RpcProcessor.GetFormatter(serviceContext.Request.ContentType);
			if (formatter != null)
				serviceContext.Response.ContentType = serviceContext.Request.ContentType;

			serviceContext.Formatter = formatter;

			var ar = BeginProcessRequest(serviceContext, serviceContext.Request.Path, cb);
			LogHelper.Debug("RpcAsyncHandler.BeginProcessRequest end return"
				+ $"ar.IsCompleted: {ar.IsCompleted}"); //JsonConvert.SerializeObject(ar));

			//if (ar.IsCompleted)
			//	cb(ar);
			return ar;
		}

		private static IAsyncResult BeginProcessRequest(ServiceContext context, string requestPath, AsyncCallback cb)
		{
			LogHelper.Debug("BeginProcessReques 2");

			if (string.IsNullOrWhiteSpace(requestPath))
				throw new ArgumentException("request.AppRelativeCurrentExecutionFilePath is null or white space");

			var service = RpcServiceFactory.GetService(requestPath);

			if (service == null)
			{
				LogHelper.Debug("BeginProcessReques Can't find service " + requestPath);
				throw new ConfigException("Configuration error service not found");
			}

			try
			{
				var actionName = requestPath.Substring(service.Path.Length);
				if (string.IsNullOrEmpty(actionName))
					throw new RequestException("Bad request: not action name");

				context.Request.ActionName = actionName;
				context.Request.ServiceType = service.Type;

				try
				{
					LogHelper.Debug("RpcAsyncHandler.BeginProcessRequest start service.BeginProcessRequest(request, response, cb, requestContext) " + requestPath);
					var result = service.BeginProcessRequest(context);
					LogHelper.Debug("RpcAsyncHandler.BeginProcessRequest end service.BeginProcessRequest(request, response, cb, requestContext) " + requestPath);
					var ar = RpcAction.ToBegin(result, cb, context);
					return ar;
					//return result;
				}
				catch (RpcLiteException)
				{
					throw;
				}
				catch (Exception ex)
				{
					throw new ProcessRequestException(ex.Message, ex);
				}
			}
			catch (Exception ex)
			{
				LogHelper.Error(ex);
				throw;
			}
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

			try
			{
				if (!result.CompletedSynchronously)
				{
					RpcService.EndProcessRequest(result);
				}

			}
			catch (Exception ex)
			{
				context.Exception = ex;
			}

			var httpContext = (HttpContext)context.ExecutingContext;
			httpContext.Response.ContentType = context.Response.ContentType;
			if (context.Exception != null)
			{
				httpContext.Response.AddHeader("RpcLite-ExceptionType", context.Exception.GetType().FullName);
				httpContext.Response.AddHeader("RpcLite-ExceptionAssembly", context.Exception.GetType().Assembly.FullName);
				httpContext.Response.AddHeader("RpcLite-StatusCode", ((int)HttpStatusCode.InternalServerError).ToString());
				httpContext.Response.StatusCode = (int)HttpStatusCode.InternalServerError;

				if (context.Formatter == null)
					throw context.Exception;

				context.Formatter.Serialize(context.Response.ResponseStream, context.Exception);
			}
			else
			{
				if (context.Result != null && context.Action.HasReturnValue)
					context.Formatter.Serialize(context.Response.ResponseStream, context.Result);

				httpContext.Response.AddHeader("RpcLite-StatusCode", ((int)HttpStatusCode.OK).ToString());
			}

			LogHelper.Debug("RpcAsyncHandler.EndProcessRequest end RpcService.EndProcessRequest(result);");
		}
	}
}