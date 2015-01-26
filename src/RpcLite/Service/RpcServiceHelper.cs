using System;
using System.Collections.Generic;
using System.Linq;
using RpcLite.Config;

namespace RpcLite.Service
{
	/// <summary>
	/// 
	/// </summary>
	public class RpcServiceHelper
	{
		/// <summary>
		/// 
		/// </summary>
		public static List<ServiceInfo> Services { get { return RpcLiteConfigSection.Instance.Services; } }

		/// <summary>
		/// 
		/// </summary>
		/// <param name="requestPath"></param>
		/// <returns></returns>
		public static ServiceInfo GetService(string requestPath)
		{
			var service = Services.FirstOrDefault(it =>
				requestPath.StartsWith(it.Path, StringComparison.OrdinalIgnoreCase));
			return service;
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="name"></param>
		/// <returns></returns>
		public static ServiceInfo GetServiceByName(string name)
		{
			var service = Services
				.FirstOrDefault(it => string.Compare(it.Name, name, StringComparison.OrdinalIgnoreCase) == 0);
			return service;
		}
	}
}
