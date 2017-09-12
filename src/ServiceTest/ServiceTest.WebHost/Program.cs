using System;
using System.IO;
using System.Linq;
using System.Net;
using System.Security.Cryptography.X509Certificates;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Server.Kestrel.Https;

namespace ServiceTest.WebHost
{
	public class Program
	{
		public static void Main(string[] args)
		{
			Console.WriteLine(args?.Length);
			//Console.WriteLine(args[0]);

			//var useHttps = args?.Length > 0 && args[0] == "ssl";
			var useHttps = args?.Any(it => it == "--ssl") ?? false;
			var useTestConnectionFilter = args?.Any(it => it == "--tcf") ?? false;

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
						//options.ThreadCount = 16;
						//options.UseHttps(certificate);
						//options.Listen(IPAddress.Loopback, 5000);
						options.Listen(IPAddress.Loopback, 5001, listenOptions =>
						{
							listenOptions.UseHttps(certificate);
						});
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
					//.UseUrls("http://*:5000/rpc/" )
					.UseUrls("http://*:5000/")
					.UseKestrel((options) =>
					{
						//options.ThreadCount = 32;
						//if (useTestConnectionFilter)
						//	options.UseHttpsTest();
						//HttpsConnectionFilterExtensionFunc.
					})
					.UseStartup<Startup>()
					.Build();

				host.Run();
			}
		}
	}
}