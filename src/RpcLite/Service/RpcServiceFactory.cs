using System;
using System.Collections.Generic;
using System.Linq;
using RpcLite.Config;
using RpcLite.Logging;

namespace RpcLite.Service
{
	/// <summary>
	/// 
	/// </summary>
	public class RpcServiceFactory
	{
		private readonly List<RpcService> _services = new List<RpcService>();
		private readonly AppHost _appHost;
		private readonly RpcLiteConfig _config;

		/// <summary>
		/// 
		/// </summary>
		/// <param name="host"></param>
		/// <param name="config"></param>
		public RpcServiceFactory(AppHost host, RpcLiteConfig config)
		{
			_appHost = host;
			_config = config;
		}

		/// <summary>
		/// 
		/// </summary>
		public void Initialize()
		{
			foreach (var item in _config.Services)
			{
				var typeInfo = TypeCreator.GetTypeByIdentifier(item.Type);
				if (typeInfo == null)
				{
					throw new ServiceException("can't load service type: " + item.Type);
				}

				_services.Add(new RpcService(typeInfo, _appHost)
				{
					Name = item.Name,
					Path = item.Path,
					//Type = typeInfo,
					//Filters = _host.Filters,
				});
			}
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="requestPath"></param>
		/// <returns></returns>
		private RpcService GetService(string requestPath)
		{
			var service = _services.FirstOrDefault(it =>
				requestPath.StartsWith(it.Path, StringComparison.OrdinalIgnoreCase));
			return service;
		}

		/// <summary>
		/// 
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

			var service = GetService(requestPath);
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

		///// <summary>
		///// 
		///// </summary>
		///// <param name="name"></param>
		///// <returns></returns>
		//public static RpcService GetServiceByName(string name)
		//{
		//	var service = Services
		//		.FirstOrDefault(it => string.Compare(it.Name, name, StringComparison.OrdinalIgnoreCase) == 0);
		//	return service;
		//}

	}

	/// <summary>
	/// 
	/// </summary>
	public struct MapServiceResult
	{
		/// <summary>
		/// 
		/// </summary>
		public static MapServiceResult Empty;

		/// <summary>
		/// 
		/// </summary>
		public bool IsServiceRequest;

		///// <summary>
		///// 
		///// </summary>
		//public ServiceContext ServiceContext;
	}

}
