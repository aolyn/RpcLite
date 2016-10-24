using System;

#if NETFX
using System.Configuration;
#else
using Microsoft.Extensions.Configuration;
using CoreConfig = Microsoft.Extensions.Configuration;
#endif

namespace RpcLite.Config
{
	/// <summary>
	/// 
	/// </summary>
	public class RpcInitializer
	{
		private static readonly object InitLocker = new object();
		private static bool _initialized;

#if NETFX

		/// <summary>
		/// initialize with web.config
		/// </summary>
		public static void Initialize()
		{
			lock (InitLocker)
			{
				if (_initialized)
					return;

				var config = ConfigurationManager.GetSection("RpcLite") as RpcConfig;
				Initialize(config);
				_initialized = true;
			}
		}

#else

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

#endif

		/// <summary>
		/// initialize with RpcConfig
		/// </summary>
		/// <param name="config"></param>
		public static void Initialize(RpcConfig config)
		{
			RpcManager.Initialize(config);
		}

		/// <summary>
		/// initialize with RpcConfigBuilder
		/// </summary>
		/// <param name="builder"></param>
		/// <returns></returns>
		public static void Initialize(Action<RpcConfigBuilder> builder)
		{
			lock (InitLocker)
			{
				if (_initialized)
					return;

				RpcManager.Initialize(builder);

				_initialized = true;
			}
		}

	}
}
