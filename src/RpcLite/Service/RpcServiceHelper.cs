using System;
using System.Collections.Generic;
using System.Linq;
using RpcLite.Config;

namespace RpcLite.Service
{
	internal class RpcServiceHelper
	{
		public static List<ServiceInfo> Services { get { return RpcLiteConfigSection.Instance.Services; } }

		public static ServiceInfo GetService(string requestPath)
		{
			var service = Services.FirstOrDefault(it =>
				requestPath.StartsWith(it.Path, StringComparison.OrdinalIgnoreCase));
			return service;
		}
	}
}
