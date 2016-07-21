//#define OUTPUT_SERIALIZATION_TIME

using System;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Threading.Tasks;
using RpcLite.Config;
using RpcLite.Formatters;
using RpcLite.Logging;

namespace RpcLite.Service
{
	/// <summary>
	/// 
	/// </summary>
	public static class RpcProcessor
	{
		/// <summary>
		/// get formatter by content type
		/// </summary>
		/// <param name="contentType"></param>
		/// <returns></returns>
		public static IFormatter GetFormatter(string contentType)
		{
			var formatter = FormatterManager.GetFormatter(contentType);
			if (formatter == null)
				throw new ConfigException("Not Supported MIME: " + contentType);
			return formatter;
		}

		/// <summary>
		/// process request
		/// </summary>
		/// <param name="serviceContext"></param>
		/// <returns></returns>
		private static Task ProcessAsync(ServiceContext serviceContext)
		{
			try
			{
				//get formatter from content type
				var formatter = GetFormatter(serviceContext.Request.ContentType);
				if (formatter != null)
					serviceContext.Response.ContentType = serviceContext.Request.ContentType;

				serviceContext.Formatter = formatter;

				var ar = ProcessRequestAsync(serviceContext);

				LogHelper.Debug("RpcAsyncHandler.BeginProcessRequest end return"
					+ $"ar.IsCompleted: {ar.IsCompleted}");

				return ar;
			}
			catch (Exception ex)
			{
				serviceContext.Exception = ex;
#if NETCORE
				return Task.FromResult<object>(null);
#else
				return Task.Factory.StartNew(() => (object)null);
#endif
			}
		}

		private static Task ProcessRequestAsync(ServiceContext context)
		{
			LogHelper.Debug("BeginProcessReques 2");

			var requestPath = context.Request.Path;

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
					var result = service.ProcessRequestAsync(context);
					LogHelper.Debug("RpcAsyncHandler.BeginProcessRequest end service.BeginProcessRequest(request, response, cb, requestContext) " + requestPath);
					return result;
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
		/// <param name="httpContext"></param>
		/// <returns>if true process processed or else the path not a service path</returns>
		public static Task<bool> ProcessAsync(IServerContext httpContext)
		{
			var tcs = new TaskCompletionSource<bool>();

			try
			{
				var isServicePath = RpcLiteConfig.Instance.ServicePaths == null
					|| (RpcLiteConfig.Instance.ServicePaths != null && RpcLiteConfig.Instance.ServicePaths
						.Any(it => httpContext.RequestPath.StartsWith(it, StringComparison.OrdinalIgnoreCase)));

				if (!isServicePath)
				{
					tcs.SetResult(false);
				}
				else
				{
					var serviceContext = new ServiceContext
					{
						Request = new ServiceRequest
						{
							RequestStream = httpContext.GetRequestStream(),
							Path = httpContext.RequestPath,
							ContentType = httpContext.GetRequestContentType(),
							ContentLength = httpContext.RequestContentLength,
						},
						Response = new ServiceResponse
						{
							ResponseStream = httpContext.GetResponseStream(),
						},
						ExecutingContext = httpContext,
					};

					try
					{
#if DEBUG
						serviceContext.SetExtensionData("StartTime", DateTime.Now);
#endif
						var result = ProcessAsync(serviceContext);
						//#if DEBUG
						//						result = result.ContinueWith(tsk => { serviceContext.SetExtensionData("EndTime", DateTime.Now); });
						//#endif
						result.ContinueWith(tsk =>
						{
#if DEBUG
							serviceContext.SetExtensionData("EndTime", DateTime.Now);
#endif
							try
							{
								EndProcessRequest(serviceContext);
								tcs.SetResult(true);
							}
							catch (Exception)
							{
								tcs.SetResult(true);
							}
						});
					}
					catch (Exception ex)
					{
						LogHelper.Error("process request error in RpcProcessor", ex);
						tcs.SetResult(true);
					}
				}
			}
			catch (Exception ex)
			{
				LogHelper.Error(ex.ToString());
				//throw;
				tcs.SetResult(false);
			}
			return tcs.Task;
		}

		private static void EndProcessRequest(ServiceContext context)
		{
			//LogHelper.Debug("RpcAsyncHandler.EndProcessRequest start RpcService.EndProcessRequest(result);");

			var httpContext = context.ExecutingContext as IServerContext;

			//var context = result.AsyncState as ServiceContext;
			if (httpContext == null)
			{
				//LogHelper.Error("ServiceContext is null", null);
				return;
			}

			//var httpContext = (HttpContext)context.ExecutingContext;
			//httpContext.Response.ContentType = context.Response.ContentType;
			httpContext.SetResponseContentType(context.Response.ContentType);

#if DEBUG
			var startTimeObj = context.GetExtensionData("StartTime");
			var endTimeObj = context.GetExtensionData("EndTime");
			if (startTimeObj != null && endTimeObj != null)
			{
				httpContext.SetResponseHeader("RpcLite-ExecutionDuration",
					((DateTime)endTimeObj - (DateTime)startTimeObj).TotalMilliseconds.ToString(CultureInfo.InvariantCulture));
			}
#endif

			if (context.Exception != null)
			{
				httpContext.SetResponseHeader("RpcLite-ExceptionType", context.Exception.GetType().FullName);
				httpContext.SetResponseHeader("RpcLite-ExceptionAssembly", context.Exception.GetType()
#if NETCORE
					.GetTypeInfo()
#endif
					.Assembly.FullName);
				httpContext.SetResponseHeader("RpcLite-StatusCode", ((int)HttpStatusCode.InternalServerError).ToString());
				//httpContext.SetResponseStatusCode((int)HttpStatusCode.InternalServerError);

				if (context.Formatter == null)
					throw context.Exception;

				//var stream = new MemoryStream();
				//context.Formatter.Serialize(stream, context.Exception);

#if OUTPUT_SERIALIZATION_TIME
				var serializationStopwatch = Stopwatch.StartNew();
#endif
				context.Formatter.Serialize(context.Response.ResponseStream, context.Exception);
#if OUTPUT_SERIALIZATION_TIME
				serializationStopwatch.Stop();
				Console.WriteLine("serializationStopwatch.ElapsedMilliseconds {0}", serializationStopwatch.ElapsedMilliseconds);
#endif
			}
			else
			{
				httpContext.SetResponseHeader("RpcLite-StatusCode", ((int)HttpStatusCode.OK).ToString());

				if (context.Result != null)
				{
					if (context.Request.RequestType == RequestType.MetaData)
					{
						if (string.IsNullOrWhiteSpace(context.Request.ContentType))
						{
							httpContext.SetResponseContentType("text/html");
							using (var writer = new StreamWriter(context.Response.ResponseStream))
							{
								var value = context.Result.ToString();
								var html = WebUtility.HtmlEncode(value);
								html = html
									.Replace(" ", "&nbsp;")
									.Replace(Environment.NewLine, "<br />");
								writer.Write(html);
							}
						}
						else
						{
#if OUTPUT_SERIALIZATION_TIME
							var serializationStopwatch = Stopwatch.StartNew();
#endif
							context.Formatter.Serialize(context.Response.ResponseStream, context.Result);
#if OUTPUT_SERIALIZATION_TIME
							serializationStopwatch.Stop();
							Console.WriteLine("serializationStopwatch.ElapsedMilliseconds {0}", serializationStopwatch.ElapsedMilliseconds);
#endif
						}
					}
					else if (context.Action != null && context.Action.HasReturnValue)
					{
#if OUTPUT_SERIALIZATION_TIME
						var serializationStopwatch = Stopwatch.StartNew();
#endif
						context.Formatter.Serialize(context.Response.ResponseStream, context.Result);
#if OUTPUT_SERIALIZATION_TIME
						serializationStopwatch.Stop();
						Console.WriteLine("serializationStopwatch.ElapsedMilliseconds {0}", serializationStopwatch.ElapsedMilliseconds);
#endif
					}
				}
			}

			//LogHelper.Debug("RpcAsyncHandler.EndProcessRequest end RpcService.EndProcessRequest(result);");
		}

	}
}