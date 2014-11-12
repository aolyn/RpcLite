//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Threading;
//using System.Web;
//using Newtonsoft.Json;
//using RpcLite.Config;
//using RpcLite.Formatters;

//namespace RpcLite.Service
//{
//	/// <summary>
//	/// 
//	/// </summary>
//	public class RpcHandler : IHttpHandler
//	{
//		/// <summary>
//		/// 
//		/// </summary>
//		public static List<ServiceInfo> Services { get { return RpcLiteConfigSection.Instance.Services; } }

//		/// <summary>
//		/// 
//		/// </summary>
//		/// <param name="context"></param>
//		public void ProcessRequest(HttpContext context)
//		{
//			var request = context.Request;
//			var response = context.Response;

//			try
//			{
//				//get formatter from content type
//				IFormatter formatter;
//				if (!string.IsNullOrEmpty(request.ContentType))
//				{
//					formatter = GlobalConfig.Formaters.FirstOrDefault(it => it.SupportMimes.Contains(request.ContentType));
//					if (formatter == null)
//						throw new ConfigException("Not Supported MIME: " + request.ContentType);

//					response.ContentType = request.ContentType;
//				}
//				else
//				{
//					if (GlobalConfig.Formaters.Count == 0)
//						throw new ConfigException("Configuration error: no formatters.");

//					formatter = GlobalConfig.Formaters[0];
//					if (formatter.SupportMimes.Count == 0)
//						throw new ConfigException("Configuration error: formatter " + formatter.GetType() + " has no support MIME");

//					response.ContentType = formatter.SupportMimes[0];
//				}

//				ProcessRequest(request, response, formatter);
//			}
//			catch (ThreadAbortException)
//			{
//			}
//			catch (Exception ex)
//			{
//				//by default send Exception data to client use Json Format
//				var resultJson = JsonConvert.SerializeObject(ex);
//				response.Write(resultJson);
//				response.End();
//			}
//		}

//		private static void ProcessRequest(HttpRequest request, HttpResponse response, IFormatter formatter)
//		{
//			if (request == null) throw new ArgumentNullException("request");

//			var requestPath = request.AppRelativeCurrentExecutionFilePath;

//			if (string.IsNullOrWhiteSpace(requestPath))
//				throw new ArgumentException("request.AppRelativeCurrentExecutionFilePath is null or white space");

//			var service = Services.FirstOrDefault(it =>
//					requestPath.StartsWith(it.Path, StringComparison.OrdinalIgnoreCase));

//			if (service == null)
//			{
//				formatter.Serialize(response.OutputStream, new ConfigException("Configuration error service not found"));
//				return;
//			}

//			try
//			{
//				var actionName = requestPath.Substring(service.Path.Length);
//				if (string.IsNullOrEmpty(actionName))
//					throw new RequestException("Bad request: not action name");

//				var serviceRequest = new ServiceRequest
//				{
//					ActionName = actionName,
//					Formatter = formatter,
//					InputStream = request.InputStream,
//					ServiceType = service.Type,
//				};

//				try
//				{
//					var result = RpcProcessor.ProcessRequest(serviceRequest);
//					formatter.Serialize(response.OutputStream, result);
//				}
//				catch (Exception ex)
//				{
//					throw new ProcessRequestException(ex.Message, ex);
//				}
//			}
//			catch (Exception ex)
//			{
//				formatter.Serialize(response.OutputStream, ex);
//			}
//		}

//		/// <summary>
//		/// 
//		/// </summary>
//		public bool IsReusable
//		{
//			get
//			{
//				return true;
//			}
//		}
//	}
//}