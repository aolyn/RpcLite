using System;
using RpcLite;
using RpcLite.Config;
using RpcLite.Server.Kestrel;
using Xunit;

namespace ServiceTest.UnitTests.Basics
{
	public class BasicTest : IDisposable
	{
		private const string HttpLocalhost = "http://localhost:50001";

		private readonly Server _server;

		public BasicTest()
		{
			_server = new ServerBuilder()
				.UseConfig(config => config.AddService<ProductService>("api/service/"))
				.UseUrls(HttpLocalhost)
				.Build();
			_server.Start();
		}

		[Fact]
		public void HostAndInvokeApiTest()
		{
			//RpcManager.Initialize(new RpcConfig());
			var appHost = new AppHost(new RpcConfig());

			var serviceAddress = HttpLocalhost + "/api/service/";
			var client = appHost.ClientFactory.GetInstance<IProductService>(serviceAddress);

			var product1 = client.GetById(9);
			Assert.NotNull(product1);
			Assert.Equal(9, product1.Id);

			var prodcut2 = client.GetByIdAsync(9).Result;
			Assert.Equal(9, prodcut2.Id);

			var page1 = client.GetPage(1, 3);
			Assert.Equal(3, page1.Length);

			var page2 = client.GetPageAsync(1, 3).Result;
			Assert.Equal(3, page2.Length);

			try
			{
				client.ExceptionTest();
				Assert.True(false);
			}
			catch (Exception ex)
			{
				Assert.Equal("test exception 235", ex.Message);
				Assert.Equal(typeof(NotImplementedException), ex.GetType());
			}
		}

		[Fact]
		public void Test1()
		{
			//RpcManager.Initialize(new RpcConfig());
			var appHost = new AppHost(new RpcConfig());

			var serviceAddress = "http://localhost:50002/api/service/";
			var client = appHost.ClientFactory.GetInstance<IProductService>(serviceAddress);

			try
			{
				client.ExceptionTest();
				Assert.True(false);
			}
			catch (Exception ex)
			{
				Assert.Equal("connection error when transport data with server", ex.Message);
				Assert.Equal(typeof(ConnectionException), ex.GetType());
			}
		}

		public void Dispose()
		{
			_server.StopAsync().GetAwaiter().GetResult();
		}
	}
}
