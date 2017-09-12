using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using ServiceTest.ServiceImpl;

namespace ServiceTest.WebHost
{
	public class Startup
	{
		public void ConfigureServices(IServiceCollection services)
		{
			services.AddMvc();
			//services.AddRouting();
		}

		// This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
		public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
		{
			loggerFactory.AddConsole(LogLevel.Error);

			////Method1: use Middleware
			//RpcLite.AspNetCore.RpcInitializer.Initialize(app);

			//RpcManager.AddFilter(new LogTimeFilter());
			//RpcManager.AddFilter(new LogRequestTimeFilter());

			//RpcLite.AspNetCore.RpcLiteInitializer.Initialize(app);

			//Method 3: use builder
			app.UseRpcLite(builder =>
			{
				builder
					.UseServicePaths("api/")
					.UseService<TestService>("TestService", "api/test/")
					.UseService<ProductService>("TestService", "api/service/")
					.UseFilter<TestFilterFactory>();
			});

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