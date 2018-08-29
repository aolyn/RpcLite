using System;
using System.Globalization;
using RpcLite.Server.Kestrel;
using Xunit;

namespace ServiceTest.UnitTests
{
	public class SelfHostSimpleTest
	{
		[Fact]
		public void Test()
		{
			var host = new HostBuilder()
				.UseConfig(config => config.AddService<Service1>("api/service/"))
				.Build();
			host.Run();
		}

		public class Service1
		{
			public string GetDateTime()
			{
				return DateTime.Now.ToString(CultureInfo.InvariantCulture);
			}
		}
	}
}
