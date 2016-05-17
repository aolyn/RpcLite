using System;
using System.Linq;
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
		/// 
		/// </summary>
		/// <param name="contentType"></param>
		/// <returns></returns>
		public static IFormatter GetFormatter(string contentType)
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

		public static Task ProcessAsync(ServiceContext serviceContext)
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

	}
}