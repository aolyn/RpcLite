using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;
using NUnit.Framework;
using RpcLite;
using RpcLite.Client;
using RpcLite.Formatters;
using RpcLite.Registry.Http;
using ServiceTest.Contract;
using ServiceTest.ServiceImpl;
using RpcLite.Config;

#if NETCORE
using Microsoft.Extensions.Configuration;
#endif

namespace ServiceTest.ClientTest_NetFx
{
	public class UnitTest
	{
		private static string serviceBaseUrl = "http://localhost:5000";

		internal static void Test()
		{
			TestHttpClient();
			//MemoryClientTest();
		}

		private static void TestHttpClient()
		{
			try
			{
				var address = serviceBaseUrl + @"/api/service/";
#if NETCORE
				var config = new RpcConfigBuilder()
					.UseClient<IProductService>("ProductService")
					.Build();

				RpcLite.AspNetCore.RpcInitializer.Initialize(config);
#else
				RpcLite.AspNet.RpcInitializer.Initialize();
#endif
				var client = ClientFactory.GetInstance<IProductService>(address);
				var clientInfo = (IRpcClient)client;
				//clientInfo.Formatter = new XmlFormatter();
				clientInfo.Format = "xml";

				TestService(client).Wait();
				TpsTest(client);
			}
			catch (Exception ex)
			{
				Console.WriteLine(ex);
			}
		}

		public static void MemoryClientTest()
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
				//.UseMonitor<ConsoleMonitorFactory>("ConsoleMonitor", "http://localhost:6201/api/service/")
				//.UseServiceMapper<DefaultServiceMapperFactory>("DefaultServiceMapper")
				.UseService<ProductService>("ProductService", path, null)
				.UseInvoker<DefaultInvokerFactory>(null)
				//.UseClient<IProductService>("ProductService", "/service/")
				.UseChannelProvider<DefaultChannelProvider>()
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
				clientInfo.Invoker = new MemoryInvoker(appHost, path);
				clientInfo.Formatter = new XmlFormatter();
			}

			TestService(client).Wait();

			TpsTest(client);
			return;

			Console.ReadLine();
		}

		private static void TpsTest(IProductService client)
		{
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
		}

		private static async Task TestService(IProductService client)
		{
			//ExceptionSerializationTest();

			Console.WriteLine("start test");

			var clientInfo = client as IRpcClient;
			if (clientInfo != null)
			{
				Console.WriteLine("formatter is " + clientInfo.Formatter?.GetType());
			}

			var testBatchName = "GetAll";
			try
			{
				var ps1 = client.GetAll();
				Assert.AreNotEqual(ps1, null);
				Assert.Greater(ps1.Length, 0);
				Assert.AreEqual(ps1.Length, 10);

				ps1 = await client.GetAllAsync();
				Assert.AreNotEqual(ps1, null);
				Assert.Greater(ps1.Length, 0);
				Assert.AreEqual(ps1.Length, 10);

				Console.WriteLine(testBatchName + " Test Success");
			}
			catch (Exception ex)
			{
				Console.BackgroundColor = ConsoleColor.Red;
				Console.WriteLine(testBatchName + " Test Failed. " + ex);
				Console.ResetColor();
			}

			testBatchName = "Add, GetById";
			try
			{
				var id1 = client.Add(new Product
				{
					Id = 1001,
					Name = "1001-name",
					Tags = new List<string> { "Tag1", "Tag2" }
				});
				Assert.AreEqual(id1, 1001);

				id1 = await client.AddAsync(new Product
				{
					Id = 1002,
					Name = "1002-name",
					Tags = new List<string> { "Tag1", "Tag2" }
				});
				Assert.AreEqual(id1, 1002);

				var product1 = client.GetById(1001);
				Assert.AreEqual(product1.Id, 1001);
				Assert.AreEqual(product1.Name, "1001-name");
				Assert.AreEqual(product1.Tags.Count, 2);
				Assert.AreEqual(product1.Tags[0], "Tag1");
				Assert.AreEqual(product1.Tags[1], "Tag2");

				var product2 = await client.GetByIdAsync(1002);
				Assert.AreEqual(product2.Id, 1002);
				Assert.AreEqual(product2.Name, "1002-name");
				Assert.AreEqual(product2.Tags.Count, 2);
				Assert.AreEqual(product2.Tags[0], "Tag1");
				Assert.AreEqual(product2.Tags[1], "Tag2");

				var id2 = client.Add(null);
				Console.WriteLine(testBatchName + " Test Success");
			}
			catch (Exception ex)
			{
				Console.BackgroundColor = ConsoleColor.Red;
				Console.WriteLine(testBatchName + " Test Failed. " + ex);
				Console.ResetColor();
			}

			testBatchName = "SetNumber, GetNumber";
			try
			{
				client.SetNumber(98);
				var num = client.GetNumber();
				Assert.AreEqual(num, 98);

				await client.SetNumberAsync(97);
				var num2 = await client.GetNumberAsync();
				Assert.AreEqual(num2, 97);
				Console.WriteLine(testBatchName + " Test Success");
			}
			catch (Exception ex)
			{
				Console.BackgroundColor = ConsoleColor.Red;
				Console.WriteLine(testBatchName + " Test Failed. " + ex);
				Console.ResetColor();
			}

			testBatchName = "GetPage";
			try
			{
				var ps1 = client.GetPage(1, 5);
				Assert.AreNotEqual(ps1, null);
				Assert.AreEqual(ps1.Length, 5);

				var ps2 = await client.GetPageAsync(1, 5);
				Assert.AreNotEqual(ps2, null);
				Assert.AreEqual(ps2.Length, 5);
				Console.WriteLine(testBatchName + " Test Success");
			}
			catch (Exception ex)
			{
				Console.BackgroundColor = ConsoleColor.Red;
				Console.WriteLine(testBatchName + " Test Failed. " + ex);
				Console.ResetColor();
			}

			testBatchName = "ThrowException";
			try
			{
				client.ExceptionTest();
			}
			catch (Exception ex)
			{
				Console.WriteLine(ex);
			}

			testBatchName = "ExceptionTestAsync";
			try
			{
				client.ExceptionTestAsync().Wait();
			}
			catch (AggregateException ex)
			{
				Console.WriteLine(ex.InnerException);
			}
			catch (Exception ex)
			{
				Console.WriteLine(ex);
			}

			var exObj = new PlatformNotSupportedException("win31");

			testBatchName = "ThrowException";
			try
			{
				client.ThrowException(exObj);
			}
			catch (Exception ex)
			{
				try
				{
					if (ex.GetType() != exObj.GetType())
					{
						Console.ForegroundColor = ConsoleColor.Yellow;
						Console.WriteLine($"Warning: Exception Type not equal: {ex.GetType()} <-> {exObj.GetType()}");
						Console.ResetColor();
					}
					else
					{
						Console.WriteLine($"Exception Type are equal: {ex.GetType()} <-> {exObj.GetType()}");
					}
					Assert.AreEqual(ex.Message, exObj.Message);
					Console.WriteLine(testBatchName + " Test Success");
				}
				catch (Exception ex2)
				{
					Console.BackgroundColor = ConsoleColor.Red;
					Console.WriteLine(testBatchName + " Test Failed. " + ex2);
					Console.ResetColor();
				}
			}

			Console.WriteLine("Test Finished.");
		}

		private static void ExceptionSerializationTest()
		{
			var formatter = new XmlFormatter();
			var ms = new MemoryStream();
			try
			{
				formatter.Serialize(ms, new NotImplementedException("test ex 23432"), typeof(Exception));
			}
			catch (Exception ex)
			{
			}

			try
			{
				var exObject = new ServiceException("test ex 234234");
				var type = exObject.GetType();
				formatter.Serialize(ms, exObject, type);
				ms.Position = 0;
				var dexObj = formatter.Deserialize(ms, type);
			}
			catch (Exception ex)
			{
			}

			try
			{
				var exObject = new NotImplementedException("test ex 23432");
				var type = exObject.GetType();
				formatter.Serialize(ms, exObject, type);
				ms.Position = 0;
				var dexObj = formatter.Deserialize(ms, type);
			}
			catch (Exception ex)
			{

			}
		}

#if NETCORE
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
#endif

	}

	class ProductServiceImp22 : RpcClientBase<IProductService>
	{
		public int GetCount()
		{
			throw new NotImplementedException();
		}

		public Task<int> GetCountAsync()
		{
			throw new NotImplementedException();
		}

		public void SetCount(int age)
		{
			throw new NotImplementedException();
		}

		public Task SetCountAsync(int age)
		{
			throw new NotImplementedException();
		}

		public Product[] GetAll()
		{
			return (Product[])base.GetResponse<Product[]>("GetAll", null, null, typeof(Product[]));
		}

		public Product[] GetPage(int pageIndex, int pageSize)
		{
			throw new NotImplementedException();
		}

		public Task<Product[]> GetAllAsync()
		{
			throw new NotImplementedException();
		}

		public virtual int Add(Product product)
		{
			return (int)GetResponse<int>("Add", product, typeof(Product), typeof(int));
		}

		public Task<int> AddAsync(Product product)
		{
			throw new NotImplementedException();
		}

		public Product GetById(int id)
		{
			throw new NotImplementedException();
		}

		public Task<Product> GetByIdAsync(int id)
		{
			throw new NotImplementedException();
		}

		public void ExceptionTest()
		{
			throw new NotImplementedException();
		}

		public void ThrowException(Exception ex)
		{
			throw new NotImplementedException();
		}

		public Task ExceptionTestAsync()
		{
			throw new NotImplementedException();
		}
	}
}