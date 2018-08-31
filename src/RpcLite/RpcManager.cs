using System.Threading.Tasks;
using RpcLite.Config;
using RpcLite.Service;

namespace RpcLite
{
	/// <summary>
	/// 
	/// </summary>
	public static class RpcManager
	{
		private static readonly object InitializeLock = new object();

		/// <summary>
		/// default AppHost
		/// </summary>
		public static AppHost AppHost { get; private set; }

		/// <summary>
		/// 
		/// </summary>
		/// <param name="config"></param>
		public static void Initialize(RpcConfig config)
		{
			if (AppHost != null)
			{
				return;
				//throw new InvalidOperationException("default service host already initialized");
			}

			lock (InitializeLock)
			{
				if (AppHost == null)
				{
					AppHost = new AppHost(config);
					//AppHost.Initialize();
				}
			}
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="serverContext"></param>
		/// <returns>if true process processed or else the path not a service path</returns>
		public static Task<bool> ProcessAsync(IServerContext serverContext)
		{
			return AppHost.ProcessAsync(serverContext);
		}
	}
}