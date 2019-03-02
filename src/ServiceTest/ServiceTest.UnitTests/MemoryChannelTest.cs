using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;
using RpcLite;
using RpcLite.Client;
using RpcLite.Formatters;
using RpcLite.Service;
using ServiceTest.UnitTests.Basics;
using Xunit;

#if NETCORE
using Microsoft.Extensions.Configuration;
#endif

namespace ServiceTest.UnitTests
{
	public class MemoryChannelTest
	{
		[Fact]
		public void MemoryClientTest()
		{
			var path = "/service/";
			var appHost = new AppHostBuilder()
				.UseAppId("10000")
				//.UseRegistry<MeropsRegistryFactory>("MeropsRegistry", "http://localhost:12974/api/service/")
				//.UseMonitor<ConsoleMonitorFactory>("ConsoleMonitor", "http://localhost:6201/api/service/")
				//.UseServiceMapper<DefaultServiceMapperFactory>("DefaultServiceMapper")
				.UseService<ProductService>("ProductService", path, null)
				.UseInvoker<DefaultInvokerFactory>(null)
				//.UseClient<IProductService>("ProductService", "/service/")
				.UseChannelProvider<DefaultChannelProvider>()
				.Build();

			//appHost.Initialize();

			//appHost.AddFilter(new LogTimeFilter());
			//appHost.AddFilter(new LogRequestTimeFilter());

			//appHost.AddFilter(new EmptyFilter());
			//appHost.AddFilter(new EmptyFilter());

			var client = appHost.ClientFactory.GetInstance<IProductService>();
			var clientInfo = client as IRpcClient;
			if (clientInfo != null)
			{
				clientInfo.Invoker = new MemoryInvoker(appHost, path);
				clientInfo.Formatter = new XmlFormatter();
			}

			TestService(client).Wait();

			//TpsTest(client);
		}

		//private void TpsTest(IProductService client)
		//{
		//	while (true)
		//	{
		//		var times = 1000;
		//		Console.WriteLine();
		//		Console.Write($"press enter to start {times} test");
		//		Console.WriteLine("testing...");

		//		var stopwatch = Stopwatch.StartNew();
		//		for (int i = 0; i < times; i++)
		//		{
		//			//var products2 = client.GetPage(1, 1000);
		//			var products22 = client.GetPage(1, 1);
		//			//var products3 = client.GetCount();
		//		}

		//		stopwatch.Stop();
		//		Console.WriteLine($"Elapsed: {stopwatch.Elapsed.TotalMilliseconds}, {times * 1000 / stopwatch.Elapsed.TotalMilliseconds} tps, {stopwatch.Elapsed.TotalMilliseconds / times}ms/t");
		//	}
		//}

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
			{
				var ps1 = client.GetAll();
				Assert.NotEqual(ps1, null);
				Assert.True(ps1.Length > 0);
				Assert.Equal(ps1.Length, 10);

				ps1 = await client.GetAllAsync();
				Assert.NotEqual(ps1, null);
				Assert.True(ps1.Length > 0);
				Assert.Equal(ps1.Length, 10);

				Console.WriteLine(testBatchName + " Test Success");
			}

			testBatchName = "Add, GetById";
			{
				var id1 = client.Add(new Product
				{
					Id = 1001,
					Name = "1001-name",
					Tags = new List<string> { "Tag1", "Tag2" }
				});
				Assert.Equal(id1, 1001);

				id1 = await client.AddAsync(new Product
				{
					Id = 1002,
					Name = "1002-name",
					Tags = new List<string> { "Tag1", "Tag2" }
				});
				Assert.Equal(id1, 1002);

				var product1 = client.GetById(1001);
				Assert.Equal(product1.Id, 1001);
				Assert.Equal(product1.Name, "1001-name");
				Assert.Equal(product1.Tags.Count, 2);
				Assert.Equal(product1.Tags[0], "Tag1");
				Assert.Equal(product1.Tags[1], "Tag2");

				var product2 = await client.GetByIdAsync(1002);
				Assert.Equal(product2.Id, 1002);
				Assert.Equal(product2.Name, "1002-name");
				Assert.Equal(product2.Tags.Count, 2);
				Assert.Equal(product2.Tags[0], "Tag1");
				Assert.Equal(product2.Tags[1], "Tag2");

				var id2 = client.Add(null);
				Console.WriteLine(testBatchName + " Test Success");
			}

			testBatchName = "SetNumber, GetNumber";
			{
				client.SetNumber(98);
				var num = client.GetNumber();
				Assert.Equal(num, 98);

				await client.SetNumberAsync(97);
				var num2 = await client.GetNumberAsync();
				Assert.Equal(num2, 97);
				Console.WriteLine(testBatchName + " Test Success");
			}

			testBatchName = "GetPage";
			{
				var ps1 = client.GetPage(1, 5);
				Assert.NotEqual(ps1, null);
				Assert.Equal(ps1.Length, 5);

				var ps2 = await client.GetPageAsync(1, 5);
				Assert.NotEqual(ps2, null);
				Assert.Equal(ps2.Length, 5);
				Console.WriteLine(testBatchName + " Test Success");
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
					Assert.Equal(ex.Message, exObj.Message);
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
}