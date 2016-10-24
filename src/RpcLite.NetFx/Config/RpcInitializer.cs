using System;
using System.Configuration;

namespace RpcLite.Config
{
	/// <summary>
	/// 
	/// </summary>
	public class RpcInitializer
	{
		private static readonly object InitLocker = new object();
		private static bool _initialized;

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
