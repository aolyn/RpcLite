using System.Threading.Tasks;
using RpcLite.Config;

namespace RpcLite.Service
{
	/// <summary>
	/// 
	/// </summary>
	public static class RpcProcessor
	{
		private static AppHost _appHost;
		private static readonly object InitilizeLock = new object();

		/// <summary>
		/// default AppHost
		/// </summary>
		public static AppHost AppHost => _appHost;

		/// <summary>
		/// 
		/// </summary>
		/// <param name="config"></param>
		public static void Initialize(RpcLiteConfig config)
		{
			if (_appHost != null)
			{
				return;
				//throw new InvalidOperationException("default service host already initialized");
			}

			lock (InitilizeLock)
			{
				if (_appHost == null)
				{
					_appHost = new AppHost(config);
					_appHost.Initialize();
				}
			}
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="httpContext"></param>
		/// <returns>if true process processed or else the path not a service path</returns>
		public static Task<bool> ProcessAsync(IServerContext httpContext)
		{
			return _appHost.ProcessAsync(httpContext);
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="filter"></param>
		public static void AddFilter(IServiceFilter filter)
		{
			_appHost.AddFilter(filter);
		}

	}
}