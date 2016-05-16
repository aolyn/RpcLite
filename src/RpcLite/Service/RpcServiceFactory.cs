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
		private static readonly List<RpcService> Services = new List<RpcService>();

		///// <summary>
		///// Services
		///// </summary>
		//public static List<RpcService> Services
		//{
		//	get { return _services; }
		//}

		static RpcServiceFactory()
		{
			foreach (var item in RpcLiteConfig.Instance.Services)
			{
				var typeInfo = TypeCreator.GetTypeFromName(item.TypeName, item.AssemblyName);
				if (typeInfo == null)
				{
					throw new ServiceException("can't load service type: " + item.Type);
				}

				Services.Add(new RpcService
				{
					Name = item.Name,
					Path = item.Path,
					Type = typeInfo,
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
		public static RpcService GetService(string requestPath)
		{
			var service = Services.FirstOrDefault(it =>
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
