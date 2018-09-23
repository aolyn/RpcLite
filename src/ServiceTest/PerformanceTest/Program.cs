using System;
using System.Globalization;
using Microsoft.AspNetCore.Hosting;
using RpcLite.Server.Kestrel;

namespace PerformanceTest
{
	class Program
	{
		static void Main(string[] args)
		{
			Console.WriteLine("Hello World 2!");

			var host = new WebHostBuilder()
				.UseKestrel()
				.UseRpcLite(config => config.AddService<TimeService>("api/service/"))
				.UseUrls("http://*:8080")
				.Build();
			host.Run();
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
