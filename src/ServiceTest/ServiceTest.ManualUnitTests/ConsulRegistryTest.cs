using Aolyn.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Hosting;
using RpcLite;
using RpcLite.Config;
using RpcLite.Registry.Consul;
using RpcLite.Server.Kestrel;
using ServiceTest.UnitTests.SelfHost;
using Xunit;

namespace ServiceTest.UnitTests
{
	public class ConsulRegistryTest
	{
		[Fact]
		public void ServiceRegisterTest()
		{
			var url = "http://localhost:5004";
			var host = new WebHostBuilder()
				.UseKestrel()
				.UseUrls(url)
				.UseRpcLite(config => config
					.AddService<TimeService>("api/service/")
					.AddService<TimeService>("TimeService", "api/service/", group: "dev")
					.AddClient<ITimeService>("TimeService", group: "dev")
					.UseServerAddress(url)
					.UseRegistry<ConsulRegistryFactory>("consul",
				//"http://192.168.9.10:8500?dc22=dc1a&host2=192.168.9.11&port2=8500&ttl=10"))
				"http://localhost:18500?dc22=dc1a&host2=localhost&port2=8500&ttl=10"))
				//.ConfigureServices(services => services.AddConfigType<ServiceClientConfigurator>())
				.Build();
			host.Run();
		}

		public class ServiceClientConfigurator
		{
			[Service]
			public ITimeService ConfigClients(AppHost apphost)
			{
				return apphost.ClientFactory.GetInstance<ITimeService>();
			}
		}

		[Fact]
		public void ServiceDiscoveryTest()
		{
			var config = new RpcConfigBuilder()
				.UseRegistry<ConsulRegistryFactory>("consul",
					"http://localhost:18500?dc=dc1&host2=localhost&port2=8500&ttl=600")
				.Build();

			var registry = new ConsulRegistry(config);
			var serviceInfo = registry.LookupAsync("TimeService", "dev").Result;
		}

		[Fact]
		public void ServiceDiscoveryTest2()
		{
			var url = "http://localhost:5004";
			var host = new WebHostBuilder()
				.UseKestrel()
				.UseUrls(url)
				.UseRpcLite(config => config
					.AddService<TimeService>("api/service/")
					.AddService<TimeService>("TimeService", "api/service/", group: "dev")
					.UseServerAddress(url)
					.UseRegistry<ConsulRegistryFactory>("consul",
						"http://localhost:18500?dc=dc1&host2=localhost&port2=8500&ttl=600"))
				.Build();
			host.Run();
		}
	}
}