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
				//.UseContentRoot(Directory.GetCurrentDirectory())
				//.UseIISIntegration()
				.UseKestrel((options) =>
				{
					options.ThreadCount = 16;
				})
				.UseStartup<Startup>()
				.Build();

			host.Run();
		}
	}
}
