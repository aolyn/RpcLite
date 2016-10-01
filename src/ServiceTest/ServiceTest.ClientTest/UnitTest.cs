using System;
using System.Diagnostics;
using Microsoft.Extensions.Configuration;
using NUnit.Framework;
using RpcLite;
using RpcLite.Client;
using RpcLite.Formatters;
using RpcLite.Monitor;
using RpcLite.Registry.Http;
using ServiceTest.Contract;
using ServiceTest.ServiceImpl;

namespace ServiceTest.ClientTest
{
	public class UnitTest
	{
		internal static void Test()
		{
			Test2();
		}

		public static void Test2()
		{
			#region prepare config


			//var config = new RpcLiteConfig
			//{
			//	AppId = "10000",
			//	Registry = new RegistryConfigItem("HttpRegistry", typeof(HttpRegistryFactory), "http://localhost:12974/api/service/"),
			//	//Monitor = new MonitorConfigItem("ConsoleMonitor", typeof(HttpMonitorFactory), "http://localhost:6201/api/service/"),
			//	Monitor = new MonitorConfigItem("ConsoleMonitor", typeof(ConsoleMonitorFactory), "http://localhost:6201/api/service/"),
			//	Services = new List<ServiceConfigItem>
			//	{
			//		new ServiceConfigItem("ProductService", typeof(ProductService), "/service/"),
			//	},
			//	Clients = new List<ClientConfigItem>
			//	{
			//		new ClientConfigItem("ProductService", typeof(IProductService), "/service/"),
			//	}
			//};

			//var appHost = new AppHost(config);

			//var appHost = new AppHostBuilder()
			//	.UseAppId("10000")
			//	.UseRegistry("HttpRegistry", typeof(HttpRegistryFactory), "http://localhost:12974/api/service/")
			//	.UseMonitor("ConsoleMonitor", typeof(ConsoleMonitorFactory), "http://localhost:6201/api/service/")
			//	.UseServices(new ServiceConfigItem("ProductService", typeof(ProductService), "/service/"))
			//	.UseClients(new ClientConfigItem("ProductService", typeof(IProductService), "/service/"))
			//	.Build();
			#endregion

			////var config1 = new ConfigurationBuilder()
			////	.AddJsonFile("rpclite.config.json")
			////	.Build();
			////var config2 = RpcConfigHelper.GetConfig(new CoreConfiguration(config1));

			var path = "/service/";
			var appHost = new AppHostBuilder()
				.UseAppId("10000")
				.UseRegistry<HttpRegistryFactory>("HttpRegistry", "http://localhost:12974/api/service/")
				.UseMonitor<ConsoleMonitorFactory>("ConsoleMonitor", "http://localhost:6201/api/service/")
				//.UseServiceMapper<DefaultServiceMapperFactory>("DefaultServiceMapper")
				.UseService<ProductService>("ProductService", path, null)
				.UseInvoker<SimpleInvokerFactory>(null)
				//.UseClient<IProductService>("ProductService", "/service/")
				.Build();

			appHost.Initialize();

			//appHost.AddFilter(new LogTimeFilter());
			//appHost.AddFilter(new LogRequestTimeFilter());

			//appHost.AddFilter(new EmptyFilter());
			//appHost.AddFilter(new EmptyFilter());


			var client = appHost.ClientFactory.GetInstance<IProductService>();
			var clientInfo = client as IRpcClient<IProductService>;
			if (clientInfo != null)
			{
				clientInfo.Invoker = new MemoryInvoker<IProductService>(appHost, path);
				clientInfo.Formatter = new XmlFormatter();
			}

			Console.WriteLine("start test");

			try
			{
				var id1 = client.Add(new Product
				{
					Id = 1,
				});
				Assert.AreEqual(id1, 1);

				client.Add(null);
			}
			catch (Exception ex)
			{
				Console.WriteLine(ex);
			}

			var exObj = new PlatformNotSupportedException("win31");
			try
			{
				client.ThrowException(exObj);
			}
			catch (Exception ex)
			{
				//Assert.AreEqual(ex.GetType(), exObj.GetType());
				//Assert.AreEqual(ex.Message, exObj.Message);
			}

			var ps = client.GetByIdAsync(1).Result;
			Assert.AreEqual(ps.Id, 1);


			var products = client.GetAll();
			while (true)
			{
				var times = 1000;
				Console.WriteLine();
				Console.Write($"press enter to start {times} test");
				Console.ReadLine();
				Console.WriteLine("testing...");

				var stopwatch = Stopwatch.StartNew();
				for (int i = 0; i < times; i++)
				{
					//var products2 = client.GetPage(1, 1000);
					var products22 = client.GetPage(1, 1);
					//var products3 = client.GetCount();
				}

				stopwatch.Stop();
				Console.WriteLine($"Elapsed: {stopwatch.Elapsed.TotalMilliseconds}, {times * 1000 / stopwatch.Elapsed.TotalMilliseconds} tps, {stopwatch.Elapsed.TotalMilliseconds / times}ms/t");
			}

			Console.ReadLine();
		}

		public static void Test1()
		{
			Console.WriteLine("start test");
			//Console.ReadLine();

			var config = new ConfigurationBuilder()
				.AddJsonFile("rpclite.config.json")
				.Build();

			var appHost = new AppHost(config);
			appHost.Initialize();

			var client = appHost.ClientFactory.GetInstance<IProductService>();

			var channel = new MemoryClientChannel(appHost) { Address = "/api/service/" };

			((IRpcClient<IProductService>)client).Invoker = new MemoryInvoker<IProductService>(appHost, "/api/service/");

			//((IRpcClient)client).Channel = channel;
			var products = client.GetAll();

			while (true)
			{
				Console.WriteLine("press enter to start 10000 test");
				Console.ReadLine();

				var stopwatch = Stopwatch.StartNew();
				var times = 10000;
				for (int i = 0; i < times; i++)
				{
					var products2 = client.GetAll();
				}

				stopwatch.Stop();
				Console.WriteLine($"Elapsed: {stopwatch.Elapsed.TotalMilliseconds}, {times * 1000 / stopwatch.Elapsed.TotalMilliseconds} tps");
			}

			Console.ReadLine();
		}
	}
}