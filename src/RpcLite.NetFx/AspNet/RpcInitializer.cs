using System;
using System.Configuration;
using RpcLite.Config;
using RpcLite.Service;

namespace RpcLite.AspNet
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
				RpcManager.Initialize(config);
				_initialized = true;
			}
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
