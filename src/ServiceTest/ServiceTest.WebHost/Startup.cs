﻿using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace ServiceTest.WebHost
{
	public class Startup
	{
		// This method gets called by the runtime. Use this method to add services to the container.
		// For more information on how to configure your application, visit http://go.microsoft.com/fwlink/?LinkID=398940
		public void ConfigureServices(IServiceCollection services)
		{
			services.AddMvc();
		}

		// This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
		public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
		{
			loggerFactory.AddConsole(LogLevel.Error);

			////Method1: use Middleware
			//RpcLiteInitializer.Initialize(app);
			//RpcManager.AddFilter(new LogTimeFilter());
			//RpcManager.AddFilter(new LogRequestTimeFilter());

			RpcLite.AspNetCore.RpcLiteInitializer.Initialize(app);

			if (env.IsDevelopment())
			{
				app.UseDeveloperExceptionPage();
			}

			//app.Run(async (context) =>
			//{
			//	await context.Response.WriteAsync("Hello World!");
			//});

			app.UseMvc(routes =>
			{
				routes.MapRoute(
					name: "default",
					template: "{controller=Home}/{action=Index}/{id?}");

				////Method2: use IRouteBuilder
				//routes.UseRpcLite();
			});
		}

	}

}
