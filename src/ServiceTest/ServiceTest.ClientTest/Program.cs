using System;
using Microsoft.Extensions.Configuration;
using RpcLite;
using RpcLite.Client;
using RpcLite.Config;
using RpcLite.Formatters;
using RpcLite.Registry;
using RpcLite.Registry.Zookeeper;
using RpcLiteClientTestNetCore;
using ServiceTest.Contract;

namespace ServiceTest.ClientTest
{
	public class Program
	{
		public static void Main(string[] args)
		{
			ConfigReadTest();
			//Test.SerializeTest.InnerExceptionTest();
			//return;
			//Test222(null);
			//UnitTest.Test();
			//ClientTest1();
			//PerformanceTest();
			//appHost.ProcessAsync()
			//UnitTest.Test();
			//RpcLite.AspNetCore.RpcLiteInitializer.Initialize();
			RegistryTest();
			//Test2();

		}

		private static void ConfigReadTest()
		{
			var configBuilder = new ConfigurationBuilder();

			//if (!string.IsNullOrWhiteSpace(basePath))
			//	configBuilder.SetBasePath(basePath);

			configBuilder
				.AddJsonFile("rpclite.config.json");

			var config = configBuilder.Build();
			var rpcConfig = RpcConfigHelper.GetConfig(new CoreConfigurationSection(config));
		}

		private static void Test222(Type type)
		{

		}

		//public static Task<ResponseMessage> SendAsync(string action, Stream content, IDictionary<string, string> headers)
		//{
		//	var response = new ResponseMessage(null);
		//	return Task.FromResult(response);
		//}

		private static void RegistryTest()
		{
			//ZookeeperRegistryTest();
			ClientTest.RegistryTest.Test();
		}

		private static void SerializationTest()
		{
			//PropertyReflectTest();

			//ReflectTest.Test();
			//JsonSerializerTester.JsonSerializerTest();

			//SerializeTest.PropertyReflectTest();
			////SerializeTest.ExceptionSerializationMultiThreadTest();
			//SerializeTest.ExceptionSerializationTest();
			//PerformanceTest();

			//SerializeTest.Test3();
			//var exobj = new Exception();
			//var setter = PropertyReflector.MakeObjectFieldSetter(PropertyReflector.GetField(typeof(Exception), "_HResult"));
			//setter(exobj, 12);

			//SerializeTest.Test3();
		}

		private static void ZookeeperRegistryTest()
		{
			var registry = new ZookeeperRegistry("192.168.9.1:2181", 10 * 1000);
			if (registry.CanRegister)
			{
				registry.RegisterAsync(new ServiceInfo
				{
					Name = "ProductService",
					Group = "UAT",
					Address = "http://localhost:5000/api/product/",
				}).Wait();
			}

			Console.WriteLine("register finished");
			Console.ReadLine();

			try
			{
				var lookupTask = registry.LookupAsync(new ClientConfigItem
				{
					Name = "ProductService",
					Group = "UAT",
				});

				Console.WriteLine("started lookup");
				Console.ReadLine();

				lookupTask.Wait();
				Console.WriteLine("lookupTask.Wait() " + lookupTask.Result);
			}
			catch (Exception ex)
			{
				Console.WriteLine(ex);
			}

			try
			{
				var lookupTask = registry.LookupAsync(new ClientConfigItem
				{
					Name = "ProductService",
					Group = "UAT",
				});

				Console.WriteLine("started lookup");
				Console.ReadLine();

				lookupTask.Wait();
				Console.WriteLine("lookupTask.Wait() " + lookupTask.Result);
			}
			catch (Exception ex)
			{
				Console.WriteLine(ex);
			}

			Console.ReadLine();
			registry.Dispose();
		}

		private static void PerformanceTest()
		{
			RpcLite.AspNetCore.RpcInitializer.Initialize();

			var client = ClientFactory.GetInstance<IProductService>(serviceBaseUrl + "/api/service/");
			try
			{
				var clientInfo = client as IRpcClient;
				Console.WriteLine(clientInfo.Address);
				Console.WriteLine(clientInfo.Formatter.ToString());

				var v01 = client.GetAll();

				var times = 5;

				Console.WriteLine("get 1 products");
				for (int idxTime = 0; idxTime < times; idxTime++)
				{
					using (new TimeRecorder())
					{
						var v1 = client.GetPage(1, 1);
					}
				}

				Console.WriteLine("get 10 products");
				for (int idxTime = 0; idxTime < times; idxTime++)
				{
					using (new TimeRecorder())
					{
						var v1 = client.GetAll();
					}
				}

				Console.WriteLine("get 100 products");
				for (int idxTime = 0; idxTime < times; idxTime++)
				{
					using (new TimeRecorder())
					{
						var v1 = client.GetPage(1, 100);
					}
				}

				Console.WriteLine("get 1000 products");
				for (int idxTime = 0; idxTime < times; idxTime++)
				{
					using (new TimeRecorder())
					{
						var v1 = client.GetPage(1, 1000);
					}
				}

			}
			catch (Exception ex)
			{
				Console.WriteLine(ex);
			}

			//Console.Write(nameof(PerformanceTest) + "finish, press enter to exit");
			//Console.ReadLine();
		}

		private static string serviceBaseUrl = "http://localhost:5000";

		private static void ClientTest1()
		{
			//var baseUrl = @"http://localhost:50001/api/service/";
			//var baseUrl = @"https://www.baidu.com/test/api/service/";
			//var baseUrl = @"http://localhost/config/asfsdfs";

			RpcLite.AspNetCore.RpcInitializer.Initialize();

			var address = serviceBaseUrl + @"/api/service/";
			var client = ClientFactory.GetInstance<IProductService>(address);
			var clientInfo = (IRpcClient)client;
			clientInfo.Formatter = new XmlFormatter();


			//var client = ClientFactory.GetInstance<IProductService>();

			try
			{
				client.ExceptionTest();
			}
			catch (Exception ex)
			{
				Console.WriteLine(ex);
			}
			//return;

			try
			{
				var products = client.GetAll();
				var products2 = client.GetAllAsync().Result;
			}
			catch (Exception ex)
			{
				Console.WriteLine(ex);
			}

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

			Console.WriteLine("Client Test Finish");

		}
	}

}