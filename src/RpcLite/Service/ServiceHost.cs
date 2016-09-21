//#define OUTPUT_SERIALIZATION_TIME

using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Net;
using System.Threading.Tasks;
using RpcLite.Config;
using RpcLite.Formatters;
using RpcLite.Logging;

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
	public class ServiceHost
	{
		private readonly RpcServiceFactory _serviceFactory;
		private readonly RpcConfig _config;
		private readonly Lazy<object> _initializeRegistry;
		private readonly AppHost _appHost;

		/// <summary>
		/// 
		/// </summary>
		public List<IServiceFilter> Filters { get; internal set; } = new List<IServiceFilter>();

		/// <summary>
		/// 
		/// </summary>
		public ServiceHost(AppHost appHost, RpcConfig config)
		{
			_config = config;
			_appHost = appHost;

			_serviceFactory = new RpcServiceFactory(_appHost, config);

			_initializeRegistry = new Lazy<object>(() =>
			{
				var services = _config?.Service?.Services;
				if (services == null || _appHost.Registry?.CanRegister != true)
					return null;

				foreach (var service in services)
				{
					_appHost.Registry.RegisterAsync(service);
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

		///// <summary>
		///// 
		///// </summary>
		///// <param name="filter"></param>
		//public void AddFilter(IRpcServiceFilter filter)
		//{
		//	Filters.Add(filter);
		//}

		///// <summary>
		///// 
		///// </summary>
		///// <param name="filter"></param>
		//public void RemoveFilter(IRpcServiceFilter filter)
		//{
		//	Filters.Remove(filter);
		//}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="serverContext"></param>
		/// <returns>if true process processed or else the path not a service path</returns>
		public Task<bool> ProcessAsync(IServerContext serverContext)
		{
			if (serverContext == null)
				throw new ArgumentNullException(nameof(serverContext));

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
			};

			return ProcessInternalAsync(serviceContext)
				.ContinueWith(tsk =>
				{
					if (tsk.Exception == null)
						return tsk.Result;

					LogHelper.Error(tsk.Exception.InnerException);
					return true;
				});
		}

		/// <summary>
		/// get formatter by content type
		/// </summary>
		/// <param name="contentType"></param>
		/// <returns></returns>
		private IFormatter GetFormatter(string contentType)
		{
			var formatter = _appHost.FormatterManager.GetFormatter(contentType);
			if (formatter == null)
				throw new ConfigException("Not Supported MIME: " + contentType);
			return formatter;
		}

		private Task<bool> ProcessInternalAsync(ServiceContext serviceContext)
		{
			try
			{
				serviceContext.Formatter = GetFormatter(serviceContext.Request.ContentType);
				serviceContext.Monitor = _appHost.Monitor?.GetSession(serviceContext);

				var parseResult = _serviceFactory.MapService(serviceContext);
				if (!parseResult.IsServiceRequest)
					return TaskHelper.FromResult(false);

#if DEBUG
				serviceContext.SetExtensionData("StartTime", DateTime.Now);
#endif
				try
				{
					serviceContext.Monitor?.BeginRequest(serviceContext);
				}
				catch (Exception ex2)
				{
					LogHelper.Error(ex2);
				}

				var result = serviceContext.Service.ProcessAsync(serviceContext);

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

					try
					{
						serviceContext.Monitor?.EndRequest(serviceContext);
					}
					catch (Exception ex2)
					{
						LogHelper.Error(ex2);
					}
					return true;
				});
			}
			catch (Exception ex)
			{
				serviceContext.Exception = ex;
				EndProcessRequest(serviceContext);

				LogHelper.Error("process request error in RpcProcessor", ex);
				return TaskHelper.FromResult(true);
			}
		}

		private void EndProcessRequest(ServiceContext context)
		{
			//LogHelper.Debug("RpcAsyncHandler.EndProcessRequest start RpcService.EndProcessRequest(result);");

			var httpContext = context.ExecutingContext;

			if (httpContext == null)
			{
				//LogHelper.Error("ServiceContext is null", null);
				return;
			}

			httpContext.ResponseContentType = context.Response.ContentType;

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
							httpContext.ResponseContentType = "text/html";
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