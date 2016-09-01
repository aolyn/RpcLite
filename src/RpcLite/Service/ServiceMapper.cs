using System;
using System.Linq;
using RpcLite.Config;
using RpcLite.Logging;

namespace RpcLite.Service
{
	/// <summary>
	/// 
	/// </summary>
	public class ServiceMapper
	{
		private readonly RpcLiteConfig _config;
		private readonly RpcServiceFactory _factory;

		/// <summary>
		/// 
		/// </summary>
		/// <param name="factory"></param>
		/// <param name="config"></param>
		public ServiceMapper(RpcServiceFactory factory, RpcLiteConfig config)
		{
			if (factory == null)
				throw new ArgumentNullException(nameof(factory));

			if (config == null)
				throw new ArgumentNullException(nameof(config));

			_config = config;
			_factory = factory;
		}

		/// <summary>
		/// <para>determinate if the request path is Service path</para>
		/// <para>get and set Service to serviceContext</para>
		/// <para>compute and set ActionName to serviceContext.Request</para>
		/// </summary>
		/// <param name="serviceContext"></param>
		/// <returns></returns>
		public MapServiceResult MapService(ServiceContext serviceContext)
		{
			var isServicePath = _config.ServicePaths == null
				|| _config.ServicePaths
					.Any(it => serviceContext.Request.Path.StartsWith(it, StringComparison.OrdinalIgnoreCase));

			if (!isServicePath)
				return MapServiceResult.Empty;

			var requestPath = serviceContext.Request.Path;
			if (string.IsNullOrWhiteSpace(requestPath))
				throw new ArgumentException("request.AppRelativeCurrentExecutionFilePath is null or white space");

			var service = _factory.Services.FirstOrDefault(it =>
				requestPath.StartsWith(it.Path, StringComparison.OrdinalIgnoreCase));
			if (service == null)
			{
				LogHelper.Debug("BeginProcessReques Can't find service " + requestPath);
				throw new ServiceNotFoundException(requestPath);
			}
			var actionName = requestPath.Substring(service.Path.Length);

			serviceContext.Request.ActionName = actionName;
			serviceContext.Service = service;

			return new MapServiceResult
			{
				IsServiceRequest = true,
				//ServiceContext = serviceContext,
			};
		}

	}
}
