using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Web;
using Newtonsoft.Json;
using RpcLite.Config;
using RpcLite.Formatters;

namespace RpcLite
{
	public class RpcAsyncHandler : IHttpAsyncHandler
	{
		public static List<ServiceInfo> Services { get { return RpcLiteConfigSection.Instance.Services; } }

		private static void ProcessRequest(HttpRequest request, HttpResponse response, IFormatter formatter)
		{
			if (request == null) throw new ArgumentNullException("request");

			var requestPath = request.AppRelativeCurrentExecutionFilePath;

			if (string.IsNullOrWhiteSpace(requestPath))
				throw new ArgumentException("request.AppRelativeCurrentExecutionFilePath is null or white space");

			var service = Services.FirstOrDefault(it =>
					requestPath.StartsWith(it.Path, StringComparison.OrdinalIgnoreCase));

			var inputStream = request.InputStream;
			var outputStream = response.OutputStream;

			if (service == null)
			{
				formatter.Serialize(outputStream, new ConfigException("Configuration error service not found"));
				return;
			}

			var action = requestPath.Substring(service.Path.Length);

			ProcessRequest(service, formatter, action, inputStream, outputStream);
		}

		private static void ProcessRequest(ServiceInfo service, IFormatter formatter, string action, System.IO.Stream inputStream, System.IO.Stream outputStream)
		{
			try
			{
				if (string.IsNullOrEmpty(action))
					throw new RequestException("Bad request: not action name");

				var serviceRequest = new ServiceRequest
				{
					ActionName = action,
					Formatter = formatter,
					InputStream = inputStream,
					ServiceType = service.Type,
				};

				try
				{
					var result = RpcProcessor.ProcessRequest(serviceRequest);
					formatter.Serialize(outputStream, result);
				}
				catch (Exception ex)
				{
					throw new ProcessRequestException(ex.Message, ex);
				}
			}
			catch (Exception ex)
			{
				formatter.Serialize(outputStream, ex);
			}
		}

		public bool IsReusable
		{
			get
			{
				return true;
			}
		}

		public IAsyncResult BeginProcessRequest(HttpContext context, AsyncCallback cb, object extraData)
		{
			var request = context.Request;
			var response = context.Response;

			IFormatter formatter = null;
			try
			{
				//get formatter from content type
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

			if (formatter != null)
			{
				try
				{
					ProcessRequest(request, response, formatter);
				}
				catch (ThreadAbortException)
				{
				}
				catch (Exception ex)
				{
					formatter.Serialize(response.OutputStream, ex);
					response.End();
				}
			}

			return null;
		}

		public void EndProcessRequest(IAsyncResult result)
		{
			throw new NotImplementedException();
		}


		public void ProcessRequest(HttpContext context)
		{
			throw new NotImplementedException();
		}
	}
}