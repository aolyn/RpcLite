using System;
using System.IO;
using System.Security.Cryptography.X509Certificates;
using Microsoft.AspNetCore.Hosting;

namespace ServiceTest.WebHost
{
	public class Program
	{
		public static void Main(string[] args)
		{
			Console.WriteLine(args?.Length);
			//Console.WriteLine(args[0]);

			var useHttps = args?.Length > 0 && args[0] == "ssl";

			if (useHttps)
			{
				var contentRoot = Directory.GetCurrentDirectory();

				//var pfxFile = @"E:\Users\Chris\Desktop\Topic\OpenSsl\cer\localhost2\localhost.pfx";
				var pfxFile = Path.Combine(contentRoot, "ssl.pfx");
				var certificate = new X509Certificate2(pfxFile);

				var host = new WebHostBuilder()
					.UseKestrel()
					.UseContentRoot(contentRoot)
					.UseIISIntegration()
					.UseUrls("https://*:5001", "http://*:5000")
					.UseKestrel((options) =>
					{
						options.ThreadCount = 16;
						options.UseHttps(certificate);
					})
					.UseStartup<Startup>()
					.Build();

				host.Run();
			}
			else
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
}