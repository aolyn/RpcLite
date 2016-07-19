using System.IO;
using Microsoft.AspNetCore.Hosting;

namespace ServiceTest.WebHost
{
	public class Program
	{
		public static void Main(string[] args)
		{
			var host = new WebHostBuilder()
				.UseKestrel()
				.UseContentRoot(Directory.GetCurrentDirectory())
				.UseIISIntegration()
				.UseUrls("http://*:5000")
				//.UseKestrel((options) =>
				//{
				//	options.ThreadCount = 16;
				//})
				.UseStartup<Startup>()
				.Build();

			host.Run();
		}
	}
}
