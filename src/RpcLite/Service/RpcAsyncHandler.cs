using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Web;
using Newtonsoft.Json;
using RpcLite.Config;
using RpcLite.Formatters;

namespace RpcLite.Service
{
	public class RpcAsyncHandler : IHttpAsyncHandler
	{
		#region Sync
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

			var service = RpcServiceHelper.Services.FirstOrDefault(it =>
					requestPath.StartsWith(it.Path, StringComparison.OrdinalIgnoreCase));

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
					var result = RpcProcessor.ProcessRequest(serviceRequest);
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

		public bool IsReusable
		{
			get
			{
				return true;
			}
		}

		#endregion

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

				return BeginProcessRequest(request.InputStream, response.OutputStream, context, request.AppRelativeCurrentExecutionFilePath, formatter, cb);
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

		private static IAsyncResult BeginProcessRequest(Stream requestStream, Stream responseStream, object requestContext, string requestPath, IFormatter formatter, AsyncCallback cb)
		{
			if (requestStream == null) throw new ArgumentNullException("requestStream");

			//var requestPath = request.AppRelativeCurrentExecutionFilePath;

			if (string.IsNullOrWhiteSpace(requestPath))
				throw new ArgumentException("request.AppRelativeCurrentExecutionFilePath is null or white space");

			var service = RpcServiceHelper.Services.FirstOrDefault(it =>
					requestPath.StartsWith(it.Path, StringComparison.OrdinalIgnoreCase));

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

				var serviceRequest = new ServiceRequest
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
					var result = RpcProcessor.BeginProcessRequest(serviceRequest, response, cb, requestContext);
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

		public void EndProcessRequest(IAsyncResult result)
		{
			var state = result.AsyncState as SeviceInvokeContext;

			if (state == null) return;

			var service = (ServiceInstanceContainer)state.Service;
			if (service == null) return;

			try
			{
				if (state.Action.HasReturnValue)
				{
					object requestResult;

					try
					{
						requestResult = state.Action.EndFunc(service.ServiceObject, result);
					}
					catch (Exception ex)
					{
						requestResult = ex;
					}
					state.Output.Formatter.Serialize(state.Output.ResponseStream, requestResult);
				}
				else
				{
					state.Action.EndAction(service.ServiceObject, result);
				}
			}
			catch (Exception ex)
			{
				state.Output.Formatter.Serialize(state.Output.ResponseStream, ex);
			}
			service.Dispose();
		}
	}

	class RpcServiceHelper
	{
		public static List<ServiceInfo> Services { get { return RpcLiteConfigSection.Instance.Services; } }
	}

}