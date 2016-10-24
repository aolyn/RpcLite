using System;
using Microsoft.Extensions.Configuration;
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
		/// initialize with RpcConfigBuilder
		/// </summary>
		/// <param name="builder"></param>
		/// <returns></returns>
		public static void Initialize(Action<RpcConfigBuilder> builder)
		{
			if (builder == null)
			{
				Initialize(new RpcConfig());
				return;
			}

			var builderObj = new RpcConfigBuilder();
			builder(builderObj);
			var config = builderObj.Build();

			Initialize(config);
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="rpcConfig"></param>
		public static void Initialize(RpcConfig rpcConfig)
		{
			RpcManager.Initialize(rpcConfig);
		}

		internal static IConfigurationRoot GetConfiguration(string basePath)
		{
			var configBuilder = new ConfigurationBuilder();

			if (!string.IsNullOrWhiteSpace(basePath))
				configBuilder.SetBasePath(basePath);

			configBuilder
				.AddJsonFile("rpclite.config.json", true);

			var config = configBuilder.Build();
			return config;
		}
	}

}
