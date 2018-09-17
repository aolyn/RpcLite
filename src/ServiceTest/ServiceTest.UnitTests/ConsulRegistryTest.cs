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
					.UseServerAddress(url)
					.UseRegistry<ConsulRegistryFactory>("consul",
						"http://localhost:18500?dc=dc1&host2=localhost&port2=8500"))
				.Build();
			host.Run();
		}
	}
}
