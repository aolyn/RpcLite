//#define OUTPUT_SERIALIZATION_TIME

using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using RpcLite.Config;
using RpcLite.Formatters;
using RpcLite.Logging;
using RpcLite.Registry;
#if OUTPUT_SERIALIZATION_TIME
using System.Diagnostics;
#endif
#if NETCORE
using System.Reflection;

#endif

namespace RpcLite.Service
{
	/// <summary>
	/// 
	/// </summary>
	public class AppHost
	{
		private readonly RpcServiceFactory _serviceFactory;
		private readonly RpcLiteConfig _config;
		private readonly Lazy<object> _initializeRegistry;

		public RegistryManager RegistryManager { get; private set; }

		/// <summary>
		/// 
		/// </summary>
		public List<IServiceFilter> Filters { get; internal set; } = new List<IServiceFilter>();

		/// <summary>
		/// 
		/// </summary>
		public string ApplicationId { get; private set; }

		/// <summary>
		/// 
		/// </summary>
		public AppHost(RpcLiteConfig config)
		{
			_config = config;
			_serviceFactory = new RpcServiceFactory(this, config);
			ApplicationId = config.AppId;

			RegistryManager = new RegistryManager(config);

			_initializeRegistry = new Lazy<object>(() =>
			{
				if (_config?.Services != null)
				{
					foreach (var service in _config.Services)
					{
						RegistryManager.Register(service);
					}
				}

				return null;
			});
		}

		/// <summary>
		/// initialize service host
		/// </summary>
		public void Initialize()
		{
			_serviceFactory.Initialize();

			// ReSharper disable once UnusedVariable
			var initilizeResult = _initializeRegistry.Value;
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="filter"></param>
		public void AddFilter(IServiceFilter filter)
		{
			Filters.Add(filter);
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="filter"></param>
		public void RemoveFilter(IServiceFilter filter)
		{
			Filters.Remove(filter);
		}

		/// <summary>
		/// get formatter by content type
		/// </summary>
		/// <param name="contentType"></param>
		/// <returns></returns>
		private IFormatter GetFormatter(string contentType)
		{
			var formatter = FormatterManager.GetFormatter(contentType);
			if (formatter == null)
				throw new ConfigException("Not Supported MIME: " + contentType);
			return formatter;
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="serverContext"></param>
		/// <returns>if true process processed or else the path not a service path</returns>
		public Task<bool> ProcessAsync(IServerContext serverContext)
		{
			if (serverContext == null)
				throw new ArgumentNullException(nameof(serverContext));

			var tcs = new TaskCompletionSource<bool>();

			try
			{
				var isServicePath = _config.ServicePaths == null
					|| (_config.ServicePaths != null && _config.ServicePaths
						.Any(it => serverContext.RequestPath.StartsWith(it, StringComparison.OrdinalIgnoreCase)));

				if (!isServicePath)
				{
					tcs.SetResult(false);
				}
				else
				{
					ProcessInternalAsync(serverContext).ContinueWith(tsk => tcs.SetResult(true));
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

		private Task ProcessInternalAsync(IServerContext serverContext)
		{
			var serviceContext = new ServiceContext
			{
				Request = new ServiceRequest
				{
					RequestStream = serverContext.RequestStream,
					Path = serverContext.RequestPath,
					ContentType = serverContext.RequestContentType,
					ContentLength = serverContext.RequestContentLength,
				},
				Response = new ServiceResponse
				{
					ResponseStream = serverContext.ResponseStream,
				},
				ExecutingContext = serverContext,
				//Formatter = formatter,
			};

			try
			{
				serviceContext.Formatter = GetFormatter(serviceContext.Request.ContentType);

				var requestPath = serverContext.RequestPath;
				if (string.IsNullOrWhiteSpace(requestPath))
					throw new ArgumentException("request.AppRelativeCurrentExecutionFilePath is null or white space");

				var service = _serviceFactory.GetService(requestPath);
				if (service == null)
				{
					LogHelper.Debug("BeginProcessReques Can't find service " + requestPath);
					throw new ServiceNotFoundException(requestPath);
				}

				var actionName = requestPath.Substring(service.Path.Length);
				serviceContext.Request.ActionName = actionName;
				serviceContext.Service = service;

#if DEBUG
				serviceContext.SetExtensionData("StartTime", DateTime.Now);
#endif
				var result = service.ProcessAsync(serviceContext);

				return result.ContinueWith(tsk =>
				{
#if DEBUG
					serviceContext.SetExtensionData("EndTime", DateTime.Now);
#endif
					try
					{
						EndProcessRequest(serviceContext);
					}
					catch (Exception ex)
					{
						LogHelper.Error(ex);
					}
				});
			}
			catch (Exception ex)
			{
				serviceContext.Exception = ex;
				EndProcessRequest(serviceContext);

				LogHelper.Error("process request error in RpcProcessor", ex);
				return TaskHelper.FromResult<object>(null);
			}
		}

		private void EndProcessRequest(ServiceContext context)
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
			httpContext.ResponseContentType = (context.Response.ContentType);

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
							httpContext.ResponseContentType = ("text/html");
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