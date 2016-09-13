using System.Configuration;
using RpcLite.Config;
using RpcLite.Service;

namespace RpcLite.AspNet
{
	/// <summary>
	/// 
	/// </summary>
	public class RpcLiteInitializer
	{
		private static readonly object InitLocker = new object();
		private static bool _initialized;

		/// <summary>
		/// 
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

	}
}
