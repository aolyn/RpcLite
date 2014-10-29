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
				IFormatter formatter = GetFormatter(request.ContentType);
				if (formatter != null)
					response.ContentType = request.ContentType;

				return ProcessRequestAsync(request.InputStream, response.OutputStream, context, request.AppRelativeCurrentExecutionFilePath, formatter, cb);
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

		private static IFormatter GetFormatter(string contentType)
		{
			IFormatter formatter;
			if (!string.IsNullOrEmpty(contentType))
			{
				formatter = GlobalConfig.Formaters.FirstOrDefault(it => it.SupportMimes.Contains(contentType));
				if (formatter == null)
					throw new ConfigException("Not Supported MIME: " + contentType);
			}
			else
			{
				if (GlobalConfig.Formaters.Count == 0)
					throw new ConfigException("Configuration error: no formatters.");

				formatter = GlobalConfig.Formaters[0];
				if (formatter.SupportMimes.Count == 0)
					throw new ConfigException("Configuration error: formatter " + formatter.GetType() + " has no support MIME");
			}
			return formatter;
		}

		private static IAsyncResult ProcessRequestAsync(Stream requestStream, Stream responseStream, object requestContext, string requestPath, IFormatter formatter, AsyncCallback cb)
		{
			if (requestStream == null) throw new ArgumentNullException("request");

			//var requestPath = request.AppRelativeCurrentExecutionFilePath;

			if (string.IsNullOrWhiteSpace(requestPath))
				throw new ArgumentException("request.AppRelativeCurrentExecutionFilePath is null or white space");

			var service = RpcServiceHelper.Services.FirstOrDefault(it =>
					requestPath.StartsWith(it.Path, StringComparison.OrdinalIgnoreCase));

			//if (service == null)
			//{
			//	formatter.Serialize(responseStream, new ConfigException("Configuration error service not found"));
			//	return;
			//}

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
					//formatter.Serialize(responseStream, result);
					return result;
				}
				catch (Exception ex)
				{
					throw new ProcessRequestException(ex.Message, ex);
				}
			}
			catch (Exception ex)
			{
				//formatter.Serialize(responseStream.OutputStream, ex);
				var result = new ServiceAsyncResult
				{
					HttpContext = requestContext,
					AsyncWaitHandle = null,
					CompletedSynchronously = true,
					IsCompleted = true,
					AsyncState = null,
				};

				cb(result);
				return result;
			}
		}

		public void EndProcessRequest(IAsyncResult result)
		{
			var state = result.AsyncState as SeviceInvokeContext;
			if (state != null)
			{
				if (state.Action.HasReturnValue)
				{
					var service = (ServiceInstanceContainer)state.Service;
					try
					{
						var requestResult = state.Action.EndFunc(service.ServiceObject, result);
						state.Output.Formatter.Serialize(state.Output.ResponseStream, requestResult);
					}
					catch (Exception ex)
					{

						throw;
					}
				}
				else
				{
					//state.Action.EndAction(state.Service, result);
				}
			}
		}
	}

	class RpcServiceHelper
	{
		public static List<ServiceInfo> Services { get { return RpcLiteConfigSection.Instance.Services; } }

	}

}