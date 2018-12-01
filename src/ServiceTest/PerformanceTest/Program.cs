using System;
using System.Globalization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using RpcLite.Config;
using RpcLite.Server.Kestrel;

namespace PerformanceTest
{
	class Program
	{
		static void Main(string[] args)
		{
			//var server = new SimpleWebServer.Server(new IPEndPoint(IPAddress.Any, 8081));
			//server.Start();
			//Console.WriteLine("Hello World!");
			//Console.ReadLine();

			Console.WriteLine("Hello World 3!");

			var config = GetCommandLineConfiguration(args);

			if (config["tv"] == "v1")
			{
				Console.WriteLine("TestWebHostBuilder");
				TestWebHostBuilder();
			}
			else
			{
				TestServerBuilder();
			}
		}

		private static IConfigurationRoot GetCommandLineConfiguration(string[] args)
		{
			return new ConfigurationBuilder()
				.AddCommandLine(args)
				.AddEnvironmentVariables()
				.Build();
		}

		private static void TestWebHostBuilder()
		{
			var host = new WebHostBuilder()
				.UseKestrel()
				//.UseConfiguration(new ConfigurationBuilder()
				//	.AddEnvironmentVariables()
				//	.AddCommandLine(Environment.GetCommandLineArgs())
				//	.Build())
				.UseLibuv()
				.UseRpcLite(config => config.AddService<TimeService>("TimeService", "api/service/",
					lifecycle: ServiceLifecycle.Singleton))
				.UseUrls("http://*:8080")
				.Build();
			host.Run();
		}

		private static void TestServerBuilder()
		{
			var host1 = new ServerBuilder()
				.UseUrls("http://*:8080")
				.UseConfig(config => config.AddService<TimeService>("TimeService", "api/service/",
					lifecycle: ServiceLifecycle.Singleton))
				.Build();
			host1.Run();
		}

		public class TimeService
		{
			public string GetDateTime()
			{
				return DateTime.Now.ToString(CultureInfo.InvariantCulture);
			}
		}
	}
}