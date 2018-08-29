using System;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using CoreConfig = Microsoft.Extensions.Configuration;

namespace RpcLite.Config
{
	/// <summary>
	/// 
	/// </summary>
	public class RpcInitializer
	{
		/// <summary>
		/// initialize with default config file "rpclite.config.json"
		/// </summary>
		public static void Initialize()
		{
			var config = GetConfiguration(null);
			Initialize(config);
		}

		/// <summary>
		/// initialize from configuration
		/// </summary>
		/// <param name="config"></param>
		public static void Initialize(CoreConfig.IConfiguration config)
		{
			var rpcConfig = RpcConfigHelper.GetConfig(new CoreConfigurationSection(config));

			Initialize(rpcConfig);
		}

		/// <summary>
		/// get IConfigurationRoot from rpclite.config.json
		/// </summary>
		/// <param name="basePath"></param>
		/// <returns></returns>
		public static IConfigurationRoot GetConfiguration(string basePath)
		{
			var configBuilder = new ConfigurationBuilder();

			if (!string.IsNullOrWhiteSpace(basePath))
				configBuilder.SetBasePath(basePath);

			configBuilder
				.AddJsonFile("rpclite.config.json", true);

			var config = configBuilder.Build();
			return config;
		}

		/// <summary>
		/// initialize with RpcConfig
		/// </summary>
		/// <param name="config"></param>
		public static void Initialize(RpcConfig config)
		{
			RpcManager.Initialize(config);
		}

#if NETCORE
		/// <summary>
		/// 
		/// </summary>
		/// <param name="config"></param>
		/// <param name="services"></param>
		public static void Initialize(RpcConfig config, IServiceCollection services)
		{
			RpcManager.Initialize(config, services);
		}
#endif

		/// <summary>
		/// initialize with RpcConfigBuilder
		/// </summary>
		/// <param name="builder"></param>
		/// <returns></returns>
		public static void Initialize(Action<RpcConfigBuilder> builder)
		{
			RpcManager.Initialize(builder);
		}
	}
}
