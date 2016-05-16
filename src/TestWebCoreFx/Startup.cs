using Microsoft.AspNet.Builder;
using Microsoft.AspNet.Hosting;
using Microsoft.AspNet.Http;
using Microsoft.Extensions.DependencyInjection;
using RpcLite.Config;

namespace TestWebCoreFx
{
	public class Startup
	{
		// This method gets called by the runtime. Use this method to add services to the container.
		// For more information on how to configure your application, visit http://go.microsoft.com/fwlink/?LinkID=398940
		public void ConfigureServices(IServiceCollection services)
		{
		}

		// This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
		public void Configure(IApplicationBuilder app)
		{
			app.UseIISPlatformHandler();

			//app.Run(async context =>
			//{
			//	await context.Response.WriteAsync("Hello World!");
			//});

			var serviceConfigItem = new ServiceConfigItem
			{
				Name = "product",
				Type = "RpcLite.TestService.RpcLiteTestService,RpcLite.CoreFx",
				TypeName = "RpcLite.TestService.RpcLiteTestService",
				AssemblyName = "RpcLite.CoreFx",

				//Type = "TestWebCoreFx.TestService,TestWebCoreFx",
				//TypeName = "TestWebCoreFx.TestService",
				//AssemblyName = "TestWebCoreFx",
				Path = "/api/test/",
			};

			var config = new RpcLiteConfig
			{
				AppId = "10000",
			};
			config.Services.Add(serviceConfigItem);
			RpcLiteConfig.SetInstance(config);

			//app.UseMiddleware<RpcLiteMiddleware>();
			app.UseRpcLiteMiddleware();
		}

		// Entry point for the application.
		public static void Main(string[] args) => WebApplication.Run<Startup>(args);
	}
}
