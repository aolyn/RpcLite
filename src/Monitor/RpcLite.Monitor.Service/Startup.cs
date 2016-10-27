using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace RpcLite.Monitor.Service
{
	public class Startup
	{
		public void ConfigureServices(IServiceCollection services)
		{
			services.AddRouting();
		}

		public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
		{
			loggerFactory.AddConsole(LogLevel.Error);

			app.UseRpcLite();

			if (env.IsDevelopment())
			{
				app.UseDeveloperExceptionPage();
			}

			app.Run(async context =>
			{
				await context.Response.WriteAsync("Hello World!");
			});
		}
	}

}
