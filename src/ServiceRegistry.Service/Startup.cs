using System.IO;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using RpcLite.Config;

namespace ServiceRegistry.Service
{
	public class Startup
	{
		IHostingEnvironment _env;
		public Startup(IHostingEnvironment env)
		{
			_env = env;
		}

		// This method gets called by the runtime. Use this method to add services to the container.
		// For more information on how to configure your application, visit http://go.microsoft.com/fwlink/?LinkID=398940
		public void ConfigureServices(IServiceCollection services)
		{
		}

		// This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
		public void Configure(IApplicationBuilder app)
		{
			ConfigurationInitializer.Initialize(app, _env.ContentRootPath);

			app.Run(async (context) =>
			{
				await context.Response.WriteAsync("Hello World!");
			});

			//var jsonFile = Path.Combine(_env.ContentRootPath, "rpclite.config.json");


		}
	}
}
