using System;
using System.Collections.Generic;
using System.Linq;
using RpcLite.Config;

namespace RpcLite.Service
{
	/// <summary>
	/// 
	/// </summary>
	public class RpcServiceFactory
	{
		private readonly List<RpcService> _services = new List<RpcService>();

		///// <summary>
		///// Services
		///// </summary>
		//public static List<RpcService> Services
		//{
		//	get { return _services; }
		//}

		//static RpcServiceFactory()
		//{
		//	Initialize();
		//}

		public void Initialize()
		{
			foreach (var item in RpcLiteConfig.Instance.Services)
			{
				var typeInfo = TypeCreator.GetTypeFromName(item.TypeName, item.AssemblyName);
				if (typeInfo == null)
				{
					throw new ServiceException("can't load service type: " + item.Type);
				}

				_services.Add(new RpcService(typeInfo)
				{
					Name = item.Name,
					Path = item.Path,
					//Type = typeInfo,
					Filters = RpcProcessor.Filters,
				});
			}
		}

		///// <summary>
		///// 
		///// </summary>
		//public static List<RpcService> Services { get { return RpcLiteConfigSection.Instance.Services; } }

		/// <summary>
		/// 
		/// </summary>
		/// <param name="requestPath"></param>
		/// <returns></returns>
		public RpcService GetService(string requestPath)
		{
			var service = _services.FirstOrDefault(it =>
				requestPath.StartsWith(it.Path, StringComparison.OrdinalIgnoreCase));
			return service;
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
}
