using System;
using System.IO;
using System.Linq;
using System.Threading;
using System.Web;
using Newtonsoft.Json;
using RpcLite.Config;
using RpcLite.Formatters;

namespace RpcLite.Service
{
	/// <summary>
	/// RpcLite AsyncHandler to process service request
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
			var request = context.Request;
			var response = context.Response;

			try
			{
				//get formatter from content type
				IFormatter formatter;
				if (!string.IsNullOrEmpty(request.ContentType))
				{
					formatter = GlobalConfig.Formaters.FirstOrDefault(it => it.SupportMimes.Contains(request.ContentType));
					if (formatter == null)
						throw new ConfigException("Not Supported MIME: " + request.ContentType);

					response.ContentType = request.ContentType;
				}
				else
				{
					if (GlobalConfig.Formaters.Count == 0)
						throw new ConfigException("Configuration error: no formatters.");

					formatter = GlobalConfig.Formaters[0];
					if (formatter.SupportMimes.Count == 0)
						throw new ConfigException("Configuration error: formatter " + formatter.GetType() + " has no support MIME");

					response.ContentType = formatter.SupportMimes[0];
				}

				ProcessRequest(request, response, formatter);
			}
			catch (ThreadAbortException)
			{
			}
			catch (Exception ex)
			{
				//by default send Exception data to client use Json Format
				var resultJson = JsonConvert.SerializeObject(ex);
				response.Write(resultJson);
				response.End();
			}
		}

		private static void ProcessRequest(HttpRequest request, HttpResponse response, IFormatter formatter)
		{
			if (request == null) throw new ArgumentNullException("request");

			var requestPath = request.AppRelativeCurrentExecutionFilePath;

			if (string.IsNullOrWhiteSpace(requestPath))
				throw new ArgumentException("request.AppRelativeCurrentExecutionFilePath is null or white space");

			var service = RpcServiceHelper.GetService(requestPath);

			if (service == null)
			{
				formatter.Serialize(response.OutputStream, new ConfigException("Configuration error service not found"));
				return;
			}

			try
			{
				var actionName = requestPath.Substring(service.Path.Length);
				if (string.IsNullOrEmpty(actionName))
					throw new RequestException("Bad request: not action name");

				var serviceRequest = new ServiceRequest
				{
					ActionName = actionName,
					Formatter = formatter,
					InputStream = request.InputStream,
					ServiceType = service.Type,
				};

				try
				{
					var result = service.ProcessRequest(serviceRequest);
					formatter.Serialize(response.OutputStream, result);
				}
				catch (Exception ex)
				{
					throw new ProcessRequestException(ex.Message, ex);
				}
			}
			catch (Exception ex)
			{
				formatter.Serialize(response.OutputStream, ex);
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
			var request = context.Request;
			var response = context.Response;

			try
			{
				//get formatter from content type
				var formatter = RpcProcessor.GetFormatter(request.ContentType);
				if (formatter != null)
					response.ContentType = request.ContentType;

				return BeginProcessRequest(request.InputStream, response.OutputStream, request.AppRelativeCurrentExecutionFilePath, formatter, cb, context);
			}
			catch (ThreadAbortException)
			{
				return new ServiceAsyncResult
				{
					AsyncState = extraData,
					IsCompleted = true,
					CompletedSynchronously = true,
					AsyncWaitHandle = null,
					HttpContext = HttpContext.Current,
				};
			}
			catch (Exception ex)
			{
				//by default send Exception data to client use Json Format
				var resultJson = JsonConvert.SerializeObject(ex);
				response.Write(resultJson);
				response.End();
			}

			return null;
		}

		private static IAsyncResult BeginProcessRequest(Stream requestStream, Stream responseStream, string requestPath, IFormatter formatter, AsyncCallback cb, object requestContext)
		{
			if (requestStream == null) throw new ArgumentNullException("requestStream");

			if (string.IsNullOrWhiteSpace(requestPath))
				throw new ArgumentException("request.AppRelativeCurrentExecutionFilePath is null or white space");

			var service = RpcServiceHelper.GetService(requestPath);

			if (service == null)
			{
				formatter.Serialize(responseStream, new ConfigException("Configuration error service not found"));
				return new ServiceAsyncResult
				{
					AsyncState = requestContext,
					IsCompleted = true,
					CompletedSynchronously = true,
					AsyncWaitHandle = null,
					HttpContext = HttpContext.Current,
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
					var result = service.BeginProcessRequest(request, response, cb, requestContext);
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
					HttpContext = requestContext,
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
			RpcService.EndProcessRequest(result);
		}
	}
}