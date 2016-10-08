using System;
using RpcLite;
using RpcLite.AspNetCore;
using RpcLite.Client;
using RpcLite.Config;
using RpcLite.Registry.Http;
using ServiceRegistry.Contract;
using ServiceTest.Contract;

namespace ServiceTest.ClientTest
{
	public class RegistryTest
	{
		public static void Test()
		{
			HttpRegistryTest();
			//DefaultRegistryTest();
		}

		public static void HttpRegistryTest()
		{
			var config = new RpcConfigBuilder()
				.UseClient<IProductService>("ProductService", "IT", null)
				.UseClient<IServiceTestService1>("IServiceTestService1", "http://localhost:5000/api/service/")
				.UseClient<IServiceTestService2>("IServiceTestService2", "it1", "http://localhost:5000/api/service/")
				.UseRegistry<HttpRegistryFactory>(null, "http://localhost:12974/api/service/")
				.Build();
			var appHost = new AppHost(config);

			var client = appHost.ClientFactory.GetInstance<IProductService>();
			var clientInfo = (IRpcClient)client;
			var serviceAddress = clientInfo.Address;
		}

		public static void HttpRegistryClientTest()
		{
			try
			{
				RpcInitializer.Initialize();

				var client = ClientFactory.GetInstance<IRegistryService>("http://localhost:12974/api/service/");

				var resp = client.GetServiceAddressAsync(new GetServiceAddressRequest
				{
					ServiceName = "ProductService",
					//Namespace = "v1",
					Group = "IT",
				});

				Console.WriteLine(resp.Result.Address.ToString());
			}
			catch (ConnectionException ex)
			{
				Console.WriteLine(ex);
				//throw;
			}
			catch (Exception ex)
			{
				Console.WriteLine(ex);
				//throw;
			}
		}

		public static void DefaultRegistryTest()
		{
			var config = new RpcConfigBuilder()
				.UseClient<IProductService>("ProductService", "http://localhost:5000/api/service/")
				.UseClient<IServiceTestService1>("IServiceTestService1", "http://localhost:5000/api/service/")
				.UseClient<IServiceTestService2>("IServiceTestService2", "it1", "http://localhost:5000/api/service/")
				.Build();

			var appHost = new AppHost(config);

			var si1 = appHost.Registry.LookupAsync<IProductService>().Result;
			var si2 = appHost.Registry.LookupAsync("ProductService").Result;
			var si3 = appHost.Registry.LookupAsync("ProductService2").Result;

			var si4 = appHost.Registry.LookupAsync<IServiceTestService2>().Result;
			var si5 = appHost.Registry.LookupAsync("IServiceTestService2").Result;
			var si6 = appHost.Registry.LookupAsync("IServiceTestService2", "it1").Result;
			var si7 = appHost.Registry.LookupAsync("IServiceTestService2", "it2").Result;
		}

		internal interface IServiceTestService1 { }

		internal interface IServiceTestService2 { }
	}
}
