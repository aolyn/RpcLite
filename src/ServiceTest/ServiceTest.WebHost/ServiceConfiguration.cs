using Aolyn.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection;
using RpcLite;
using ServiceTest.Contract;

namespace ServiceTest.WebHost
{
	[Configuration]
	public class ServiceConfiguration
	{
		[Service(ServiceLifetime.Singleton)]
		public IProductService GetProductService(AppHost appHost)
		{
			return appHost.ClientFactory.GetInstance<IProductService>("http://localhost:5000/api/service/");
		}
	}

	[Configuration]
	public class ServiceConfiguration2
	{
		[Service(ServiceLifetime.Singleton)]
		public Bean2 GetBean2(IProductService appHost, Bean3 bean3, IBean4 bean4)
		{
			return new Bean2(); 
		}
	}

	public class Bean2
	{

	}

	[Service(ServiceLifetime.Singleton)]
	public class Bean3
	{

	}

	[Service(ServiceLifetime.Singleton, typeof(IBean4))]
	public class Bean4 : IBean4
	{

	}

	public interface IBean4
	{

	}
}