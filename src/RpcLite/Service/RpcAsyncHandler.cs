using System;
using System.IO;
using System.Threading;
using System.Web;
using RpcLite.Config;
using RpcLite.Formatters;
using RpcLite.Utility;

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

			//Hrj.Logging.Logger.Debug("BeginProcessRequest: " + request.Url);

			try
			{
				//get formatter from content type
				var formatter = RpcProcessor.GetFormatter(request.ContentType);
				if (formatter != null)
					response.ContentType = request.ContentType;

				var requestPath = request.Path;
				// ReSharper disable once PossibleNullReferenceException
				requestPath = request.ApplicationPath.Length == 1
					? "~" + requestPath
					: "~" + requestPath.Substring(request.ApplicationPath.Length);

				var ar = BeginProcessRequest(request.InputStream, response.OutputStream, requestPath, formatter, cb, context);
				//Hrj.Logging.Logger.Debug("RpcAsyncHandler.BeginProcessRequest end return"
				//+ string.Format("ar.IsCompleted: {0}", ar.IsCompleted)); //JsonConvert.SerializeObject(ar));

				//if (ar.IsCompleted)
				//	cb(ar);
				return ar;
			}
			catch (ThreadAbortException)
			{
				return new ServiceAsyncResult
				{
					AsyncState = extraData,
					IsCompleted = true,
					CompletedSynchronously = true,
					AsyncWaitHandle = null,
					//HttpContext = HttpContext.Current,
				};
			}
			catch (TypeInitializationException ex)
			{
				JsonHelper.Serialize(response.OutputStream, ex.InnerException);
				response.End();
			}
			catch (Exception ex)
			{
				//by default send Exception data to client use Json Format
				JsonHelper.Serialize(response.OutputStream, ex);
				//response.Write(resultJson);
				response.End();
			}

			return null;
		}

		private static IAsyncResult BeginProcessRequest(Stream requestStream, Stream responseStream, string requestPath, IFormatter formatter, AsyncCallback cb, object requestContext)
		{
			if (requestStream == null) throw new ArgumentNullException("requestStream");

			//Hrj.Logging.Logger.Debug("BeginProcessReques 2");

			if (string.IsNullOrWhiteSpace(requestPath))
				throw new ArgumentException("request.AppRelativeCurrentExecutionFilePath is null or white space");

			var service = RpcServiceHelper.GetService(requestPath);

			if (service == null)
			{
				//Hrj.Logging.Logger.Debug("BeginProcessReques Can't find service " + requestPath);

				formatter.Serialize(responseStream, new ConfigException("Configuration error service not found"));
				return new ServiceAsyncResult
				{
					AsyncState = requestContext,
					IsCompleted = true,
					CompletedSynchronously = true,
					AsyncWaitHandle = null,
					//HttpContext = HttpContext.Current,
				};
			}

			try
			{
				var actionName = requestPath.Substring(service.Path.Length);
				if (string.IsNullOrEmpty(actionName))
					throw new RequestException("Bad request: not action name");

				var request = new ServiceRequest
				{
					ActionName = actionName,
					Formatter = formatter,
					InputStream = requestStream,
					ServiceType = service.Type,
				};

				var response = new ServiceResponse
				{
					ResponseStream = responseStream,
					Formatter = formatter,
				};

				try
				{
					//Hrj.Logging.Logger.Debug("RpcAsyncHandler.BeginProcessRequest start service.BeginProcessRequest(request, response, cb, requestContext) " + requestPath);
					var result = service.BeginProcessRequest(request, response, cb, requestContext);
					//Hrj.Logging.Logger.Debug("RpcAsyncHandler.BeginProcessRequest end service.BeginProcessRequest(request, response, cb, requestContext) " + requestPath);
					return result;
				}
				catch (Exception ex)
				{
					throw new ProcessRequestException(ex.Message, ex);
				}
			}
			catch (Exception ex)
			{
				formatter.Serialize(responseStream, ex);
				var result = new ServiceAsyncResult
				{
					//HttpContext = requestContext,
					AsyncWaitHandle = null,
					CompletedSynchronously = true,
					IsCompleted = true,
					AsyncState = null,
				};

				return result;
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
			//Hrj.Logging.Logger.Debug("RpcAsyncHandler.EndProcessRequest start RpcService.EndProcessRequest(result);");
			RpcService.EndProcessRequest(result);
			//Hrj.Logging.Logger.Debug("RpcAsyncHandler.EndProcessRequest end RpcService.EndProcessRequest(result);");
		}
	}
}