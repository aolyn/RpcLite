using Aolyn.Config;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace ServiceRegistry.Service
{
	public class Startup
	{
		public IConfigurationRoot Configuration { get; }
		private readonly IHostingEnvironment _env;

		public Startup(IHostingEnvironment env)
		{
			var builder = new ConfigurationBuilder()
				.SetBasePath(env.ContentRootPath)
				.AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
				.AddJsonFile($"appsettings.{env.EnvironmentName}.json", optional: true)
				.AddEnvironmentVariables();
			Configuration = builder.Build();

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
		public void Configure(IApplicationBuilder app, ILoggerFactory loggerFactory)
		{
			loggerFactory.AddConsole(_env.IsProduction() ? LogLevel.Warning : LogLevel.Debug);

			app.UseRpcLite();

			app.Run(async (context) =>
			{
				await context.Response.WriteAsync("Hello World!");
			});

		}
	}
}
