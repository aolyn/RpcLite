using Microsoft.AspNetCore.Hosting;
using RpcLite.Registry.Consul;
using RpcLite.Server.Kestrel;
using Xunit;

namespace ServiceTest.UnitTests
{
	public class ConsulRegistryTest
	{
		[Fact]
		public void Test()
		{
			var url = "http://localhost:5004";
			var host = new WebHostBuilder()
				.UseKestrel()
				.UseUrls(url)
				.UseRpcLite(config => config
					.AddService<TimeService>("api/service/")
					.UseRegistry<ConsulRegistryFactory>("consul", "http://localhost:8500?dc=dc1")
					.UseServerAddress(url))
				.Build();
			host.Run();
		}
	}
}
