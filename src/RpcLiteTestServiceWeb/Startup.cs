using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using RpcLite.Config;
using RpcLite.Service;

namespace RpcLiteTestServiceWeb
{
	public class Startup
	{
		public Startup(IHostingEnvironment env)
		{
		}

		// This method gets called by the runtime. Use this method to add services to the container.
		// For more information on how to configure your application, visit http://go.microsoft.com/fwlink/?LinkID=398940
		public void ConfigureServices(IServiceCollection services)
		{
		}

		// This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
		public void Configure(IApplicationBuilder app)
		{
			ConfigurationInitializer.Initialize(app);
			//InitializeRpcLite(app);

			//app.UseMiddleware<Middleware2>();

			//app.UseRpcLiteMiddleware();
		}

		private static void InitializeRpcLite(IApplicationBuilder app)
		{
			var jsonFile = "rpclite.config.json";
			//InitializeFromJsonConfig("rpclite.config.json");
			app.UseMiddleware<RpcLiteMiddleware>();
			//}

			//public static void InitializeFromJsonConfig(string jsonFile)
			//{
			var config = new ConfigurationBuilder()
				.AddJsonFile(jsonFile)
				.Build();

			//var config = new ConfigurationBuilder()
			//	.AddXmlFile("rpclite.config.xml")
			//	.Build();

			//var rpcConfig = RpcLiteConfigurationHelper.GetConfig(new CoreConfigurationSection(config));
			//RpcLiteConfig.SetInstance(rpcConfig);

			ConfigurationInitializer.Initialize(config);
		}

	}
}