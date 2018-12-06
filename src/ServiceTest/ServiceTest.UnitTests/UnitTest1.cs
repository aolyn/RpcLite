using System;
using RpcLite;
using RpcLite.Client;
using RpcLite.Config;
using ServiceTest.Contract;
using Xunit;
using IProductService = ServiceTest.UnitTests.Basics.IProductService;

namespace ServiceTest.UnitTests
{
	public class UnitTest1 : IDisposable
	{
		public UnitTest1()
		{

		}

		[Fact]
		public void Test1()
		{
			RpcManager.Initialize(new RpcConfig());

			var serviceAddress = "http://localhost:5000/api/service/";
			var client = ClientFactory.GetInstance<IProductService>(serviceAddress);

			var product1 = client.GetById(9);
			Assert.NotNull(product1);
			Assert.Equal(9, product1.Id);

			var prodcut2 = client.GetByIdAsync(9).Result;
			Assert.Equal(9, prodcut2.Id);

			var page1 = client.GetPage(1, 3);
			Assert.Equal(3, page1.Length);

			var page2 = client.GetPageAsync(1, 3).Result;
			Assert.Equal(3, page2.Length);
		}

		public void Dispose()
		{
		}
	}
}
