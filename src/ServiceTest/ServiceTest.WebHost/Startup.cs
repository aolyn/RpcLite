using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using RpcLite.Config;
using ServiceTest.ServiceImpl;

namespace ServiceTest.WebHost
{
	public class Startup
	{
		public void ConfigureServices(IServiceCollection services)
		{
			services.AddControllersWithViews();

			services
				.Configure<KestrelServerOptions>(x => x.AllowSynchronousIO = true)
				.Configure<IISServerOptions>(x => x.AllowSynchronousIO = true);
			//services.AddMvc();
			//services.AddRouting();

			services.AddRpcLite(builder => builder
				.AddService<TestService>("TestService", "api/test/")
				.AddService<ProductService>("ProductService", "api/service/", null, ServiceLifecycle.Scoped)
				.AddService<TimeService>("TimeService", "api/time/")
				.AddFilter<TestFilterFactory>());

			services.AddConfigurationAssembly<Startup>();
		}

		// This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
		public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
		{
			//loggerFactory.AddConsole(LogLevel.Error);

			//RpcManager.AddFilter(new LogTimeFilter());
			//RpcManager.AddFilter(new LogRequestTimeFilter());

			//Method 3: use builder
			app.UseRpcLite();
			//app.UseRpcLite(builder => builder
			//	.UseService<TestService>("TestService", "api/test/")
			//	.UseService<ProductService>("ProductService", "api/service/")
			//	.UseFilter<TestFilterFactory>());

			if (env.IsDevelopment())
			{
				app.UseDeveloperExceptionPage();
			}

			//app.UseMvc(routes =>
			//{
			//	routes.MapRoute(
			//		name: "default",
			//		template: "{controller=Home}/{action=Index}/{id?}");

			//	////Method2: use IRouteBuilder
			//	//routes.UseRpcLite(builder => builder
			//	//	.UseService<TestService>("TestService", "api/test/")
			//	//	.UseService<ProductService>("TestService", "api/service/")
			//	//	.UseFilter<TestFilterFactory>());
			//});
			app.UseRouting();

			app.UseEndpoints(endpoints =>
			{
				endpoints.MapControllerRoute(
					name: "default",
					pattern: "{controller=Home}/{action=Index}/{id?}");
			});

			app.Run(async (context) =>
			{
				await context.Response.WriteAsync("Hello World!");
			});
		}
	}
}