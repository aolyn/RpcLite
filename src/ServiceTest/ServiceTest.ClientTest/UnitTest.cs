using System;
using System.Diagnostics;
using Microsoft.Extensions.Configuration;
using RpcLite;
using RpcLite.Client;
using ServiceTest.Contract;

namespace ServiceTest.ClientTest
{
	public class UnitTest
	{
		internal static void Test()
		{
			Test1();
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

			((IRpcClient)client).Channel = channel;
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
