using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using RpcLite.Config;
using RpcLite.Service;
using CoreConfig = Microsoft.Extensions.Configuration;

namespace RpcLite.AspNetCore
{
	/// <summary>
	/// 
	/// </summary>
	public class RpcLiteInitializer
	{
		/// <summary>
		/// initialize with default config file "rpclite.config.json"
		/// </summary>
		public static void Initialize()
		{
			Initialize(null, null);
		}

		/// <summary>
		/// initialize from configuration
		/// </summary>
		/// <param name="config"></param>
		public static void Initialize(CoreConfig.IConfiguration config)
		{
			var rpcConfig = RpcConfigHelper.GetConfig(new CoreConfigurationSection(config));
			//RpcLiteConfig.SetInstance(rpcConfig);

			RpcManager.Initialize(rpcConfig);
		}

		/// <summary>
		/// default initialize from rpclite.config.json
		/// </summary>
		/// <param name="app">used to UseMiddleware, keep null if not need set</param>
		public static void Initialize(IApplicationBuilder app)
		{
			//var jsonFile = "rpclite.config.json";
			Initialize(app, null);
		}

		/// <summary>
		/// <para>initialize with default config file "rpclite.config.json" in specific basePath</para>
		/// </summary>
		/// <param name="app">used to UseMiddleware, keep null if not need set</param>
		/// <param name="basePath">base path to search config file rpclite.config.json</param>
		public static void Initialize(IApplicationBuilder app, string basePath)
		{
			var configBuilder = new ConfigurationBuilder();

			if (!string.IsNullOrWhiteSpace(basePath))
				configBuilder.SetBasePath(basePath);

			configBuilder
				.AddJsonFile("rpclite.config.json");

			var config = configBuilder.Build();

			Initialize(config);

			app?.UseMiddleware<RpcLiteMiddleware>();
		}

	}
}
