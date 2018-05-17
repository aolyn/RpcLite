using System;
using System.Diagnostics;
using Microsoft.Extensions.Configuration;
using RpcLite;
using RpcLite.Client;
using RpcLite.Config;
using RpcLite.Formatters;
using RpcLite.Monitor;
using RpcLite.Registry.Merops;
using ServiceTest.Contract;
using ServiceTest.ServiceImpl;
using Xunit;

namespace ServiceTest.ClientTest
{
	public class UnitTest
	{
		internal static void Test()
		{
			Test2();
		}

		[Fact]
		public static void Test4()
		{
			var appHost = new AppHostBuilder()
				.UseAppId("10000")
				.UseInvoker<DefaultInvokerFactory>(null)
				.UseClient<IProductService>("ProductService", "http://localhost:5000/api/service/")
				.UseChannelProvider<DefaultChannelProvider>()
				.Build();
			appHost.Initialize();

			var client = appHost.ClientFactory.GetInstance<IProductService>();
			//var clientInfo = (IRpcClient<IProductService>)client;
			//clientInfo.Invoker = new MemoryInvoker(appHost, path);
			//clientInfo.Formatter = new XmlFormatter();
			//clientInfo.Format = "xml";

			Console.WriteLine("start test");

			try
			{
				var id1 = client.Add(new Product
				{
					Id = 1,
				});
				Assert.Equal(1, id1);

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
				Assert.Equal(ex.GetType(), exObj.GetType());
				Assert.Equal(ex.Message, exObj.Message);
			}

			var ps = client.GetByIdAsync(1).Result;
			Assert.Equal(1, ps.Id);


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

		[Fact]
		public static void Test2()
		{
			#region prepare config


			//var config = new RpcLiteConfig
			//{
			//	AppId = "10000",
			//	Registry = new RegistryConfigItem("MeropsRegistry", typeof(MeropsRegistryFactory), "http://localhost:12974/api/service/"),
			//	//Monitor = new MonitorConfigItem("ConsoleMonitor", typeof(MeropsMonitorFactory), "http://localhost:6201/api/service/"),
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
			//	.UseRegistry("MeropsRegistry", typeof(MeropsRegistryFactory), "http://localhost:12974/api/service/")
			//	.UseMonitor("ConsoleMonitor", typeof(ConsoleMonitorFactory), "http://localhost:6201/api/service/")
			//	.UseServices(new ServiceConfigItem("ProductService", typeof(ProductService), "/service/"))
			//	.UseClients(new ClientConfigItem("ProductService", typeof(IProductService), "/service/"))
			//	.Build();
			#endregion

			////var config1 = new ConfigurationBuilder()
			////	.AddJsonFile("rpclite.config.json")
			////	.Build();
			////var config2 = RpcConfigHelper.GetConfig(new CoreConfiguration(config1));

			var config = new RpcConfigBuilder()
				.UseAppId("10000")
				.UseRegistry<MeropsRegistryFactory>("MeropsRegistry", "http://localhost:12974/api/service/")
				.UseMonitor<ConsoleMonitorFactory>("ConsoleMonitor", "http://localhost:6201/api/service/")
				.UseInvoker<DefaultInvokerFactory>(null)
				.UseFilter<UnitTestFilterFactory>()
				.Build();
			var appHost2 = new AppHost(config);

			var path = "/service/";
			var appHost = new AppHostBuilder()
				.UseAppId("10000")
				.UseRegistry<MeropsRegistryFactory>("MeropsRegistry", "http://localhost:12974/api/service/")
				.UseMonitor<ConsoleMonitorFactory>("ConsoleMonitor", "http://localhost:6201/api/service/")
				//.UseServiceMapper<DefaultServiceMapperFactory>("DefaultServiceMapper")
				.UseService<ProductService>("ProductService", path, null)
				.UseInvoker<DefaultInvokerFactory>(null)
				//.UseClient<IProductService>("ProductService", "/service/")
				.UseFilter<UnitTestFilterFactory>()
				.Build();
			appHost.Initialize();

			//appHost.AddFilter(new LogTimeFilter());
			//appHost.AddFilter(new LogRequestTimeFilter());

			//appHost.AddFilter(new EmptyFilter());
			//appHost.AddFilter(new EmptyFilter());


			var client = appHost.ClientFactory.GetInstance<IProductService>();
			var clientInfo = (IRpcClient<IProductService>)client;
			clientInfo.Invoker = new MemoryInvoker(appHost, path);
			clientInfo.Formatter = new XmlFormatter();
			clientInfo.Format = "xml";

			Console.WriteLine("start test");

			try
			{
				var id1 = client.Add(new Product
				{
					Id = 1,
				});
				Assert.Equal(1, id1);

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
				Assert.Equal(ex.GetType(), exObj.GetType());
				Assert.Equal(ex.Message, exObj.Message);
			}

			var ps = client.GetByIdAsync(1).Result;
			Assert.Equal(1, ps.Id);


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

		private void Test234234()
		{
			var appHost = new AppHostBuilder()
				.UseAppId("10000")
				.UseRegistry<MeropsRegistryFactory>("MeropsRegistry", "http://localhost:12974/api/service/")
				//其它配置
				.Build();
			appHost.Initialize();
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

			((IRpcClient<IProductService>)client).Invoker = new MemoryInvoker(appHost, "/api/service/");

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