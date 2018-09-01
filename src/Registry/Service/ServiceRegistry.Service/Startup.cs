using Aolyn.Config;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using RpcLite.Config;

namespace ServiceRegistry.Service
{
	public class Startup
	{
		private readonly IHostingEnvironment _env;
		public Startup(IHostingEnvironment env)
		{
			_env = env;
		}

		public void ConfigureServices(IServiceCollection services)
		{
			var config = new ConfigurationBuilder()
				.AddJsonFile("appsettings.json", true)
				.Build();
			ConfigurationManager.Initialize(config);

			services.AddRouting();
			services.AddRpcLite();
		}

		// This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
		public void Configure(IApplicationBuilder app)
		{
			app.UseRpcLite();

			app.Run(async (context) =>
			{
				await context.Response.WriteAsync("Hello World!");
			});

		}
	}
}
