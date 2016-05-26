using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using RpcLite.Service;
using CoreConfig = Microsoft.Extensions.Configuration;

namespace RpcLite.Config
{
	public class ConfigurationInitializer
	{
		public static void Initialize(CoreConfig.IConfiguration config)
		{
			var rpcConfig = RpcLiteConfigurationHelper.GetConfig(new CoreConfigurationSection(config));
			RpcLiteConfig.SetInstance(rpcConfig);
		}

		/// <summary>
		/// default initialize from rpclite.config.json
		/// </summary>
		/// <param name="app"></param>
		public static void Initialize(IApplicationBuilder app)
		{
			var jsonFile = "rpclite.config.json";
			Initialize(app, jsonFile);
		}

		public static void Initialize(IApplicationBuilder app, string basePath)
		{
			var config = new ConfigurationBuilder()
				.SetBasePath(basePath)
				.AddJsonFile("rpclite.config.json")
				.Build();

			Initialize(config);

			app.UseMiddleware<RpcLiteMiddleware>();
		}
	}
}
