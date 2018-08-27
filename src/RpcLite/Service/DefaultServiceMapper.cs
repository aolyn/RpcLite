using System;
using System.Linq;
using RpcLite.Logging;

namespace RpcLite.Service
{
	/// <summary>
	/// 
	/// </summary>
	public class DefaultServiceMapper : IServiceMapper
	{
		private readonly RpcServiceFactory _factory;

		/// <summary>
		/// 
		/// </summary>
		/// <param name="factory"></param>
		public DefaultServiceMapper(RpcServiceFactory factory)
		{
			if (factory == null)
				throw new ArgumentNullException(nameof(factory));

			_factory = factory;
		}

		/// <summary>
		/// <para>determinate if the request path is Service path</para>
		/// <para>if yes get and set Service to serviceContext</para>
		/// <para>compute and set ActionName to serviceContext.Request</para>
		/// </summary>
		/// <param name="serviceContext"></param>
		/// <returns></returns>
		public MapServiceResult MapService(ServiceContext serviceContext)
		{
			//var isServicePath = _config.Service?.Paths == null
			//	|| _config.Service.Paths
			//		.Any(it => serviceContext.Request.Path.StartsWith(it, StringComparison.OrdinalIgnoreCase));

			//if (!isServicePath)
			//	return MapServiceResult.Empty;

			var requestPath = serviceContext.Request.Path;
			if (string.IsNullOrWhiteSpace(requestPath))
				throw new ArgumentException("request.AppRelativeCurrentExecutionFilePath is null or white space");

			var service = _factory.Services.FirstOrDefault(it =>
					requestPath.StartsWith(it.Path, StringComparison.OrdinalIgnoreCase));

			if (service == null)
			{
				service = _factory.Services.FirstOrDefault(it =>
					it.Path.StartsWith(requestPath, StringComparison.OrdinalIgnoreCase));
				if (service == null)
				{
					LogHelper.Debug("BeginProcessReques Can't find service " + requestPath);
					throw new ServiceNotFoundException(requestPath);
				}
			}
			else
			{
				var actionName = requestPath.Substring(service.Path.Length);
				serviceContext.Request.ActionName = actionName;
			}
			serviceContext.Service = service;

			return new MapServiceResult
			{
				IsServiceRequest = true,
				//ServiceContext = serviceContext,
			};
		}
	}

}
