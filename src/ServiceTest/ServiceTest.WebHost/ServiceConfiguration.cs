using System;
using Microsoft.Extensions.DependencyInjection;
using RpcLite;
using ServiceTest.Common;
using ServiceTest.Contract;

namespace ServiceTest.WebHost
{
	public class ServiceConfiguration
	{
		[Service(ServiceLifetime.Singleton)]
		public static IProductService GetProductService(IServiceProvider services)
		{
			var appHost = services.GetService<AppHost>();
			return appHost.ClientFactory.GetInstance<IProductService>("http://localhost:5000/api/service/");
		}
	}
}