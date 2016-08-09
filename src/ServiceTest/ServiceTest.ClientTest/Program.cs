using System;
using RpcLite.Client;
using RpcLite.Config;
using RpcLite.Registry.Zookeeper;
using RpcLiteClientTestNetCore;
using ServiceTest.ClientTest.Test;
using ServiceTest.Contract;

namespace ServiceTest.ClientTest
{
	public class Program
	{
		public static void Main(string[] args)
		{
			//ZookeeperRegistryTest();

			RpcLiteInitializer.Initialize();

			//RegistryTest.Test();

			//SerializeTest.PropertyReflectTest();
			////SerializeTest.ExceptionSerializationMultiThreadTest();
			//SerializeTest.ExceptionSerializationTest();
			//PerformanceTest();

			//SerializeTest.Test3();
			//var exobj = new Exception();
			//var setter = PropertyReflector.MakeObjectFieldSetter(PropertyReflector.GetField(typeof(Exception), "_HResult"));
			//setter(exobj, 12);

			//SerializeTest.Test3();

			Test1();
			PerformanceTest();
			//PropertyReflectTest();

			//ReflectTest.Test();
			//JsonSerializerTester.JsonSerializerTest();
			//Test2();
		}

		private static void ZookeeperRegistryTest()
		{
			var registry = new ZookeeperRegistry("192.168.9.1:2181", 10 * 1000);
			if (registry.CanRegister)
			{
				registry.RegisterAsync(new ServiceConfigItem
				{
					Name = "ProductService",
					Environment = "UAT",
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
					Environment = "UAT",
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
					Environment = "UAT",
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
			var client = ClientFactory.GetInstance<IProductService>(serviceBaseUrl + "/api/service/");
			try
			{
				var clientInfo = client as IRpcClient;
				Console.WriteLine(clientInfo.BaseUrl);
				Console.WriteLine(clientInfo.Formatter.ToString());

				Console.WriteLine("get 10 products");
				for (int idxTime = 0; idxTime < 19; idxTime++)
				{
					using (new TimeRecorder())
					{
						var v1 = client.GetAll();
					}
				}

				Console.WriteLine("get 100 products");
				for (int idxTime = 0; idxTime < 19; idxTime++)
				{
					using (new TimeRecorder())
					{
						var v1 = client.GetPage(1, 100);
					}
				}

				Console.WriteLine("get 1000 products");
				for (int idxTime = 0; idxTime < 19; idxTime++)
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
		}

		private static string serviceBaseUrl = "http://localhost:17518";

		private static void Test1()
		{
			//var baseUrl = @"http://localhost:50001/api/service/";
			//var baseUrl = @"https://www.baidu.com/test/api/service/";
			//var baseUrl = @"http://localhost/config/asfsdfs";

			var baseUrl = serviceBaseUrl + @"/api/service/";
			//var client = ClientFactory.GetInstance<IProductService>(baseUrl);
			var client = ClientFactory.GetInstance<IProductService>();


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
			catch (Exception ex)
			{
				Console.WriteLine(ex);
			}

		}
	}

}