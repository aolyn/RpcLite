using System;
using RpcLite.Client;
using RpcLiteClientTestNetCore;
using ServiceTest.ClientTest.Test;
using ServiceTest.Contract;

namespace ServiceTest.ClientTest
{
	public class Program
	{
		public static void Main(string[] args)
		{
			RpcLite.Config.RpcLiteInitializer.Initialize();

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