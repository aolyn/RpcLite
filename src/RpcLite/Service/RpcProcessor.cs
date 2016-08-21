using System.Threading.Tasks;
using RpcLite.Config;

namespace RpcLite.Service
{
	/// <summary>
	/// 
	/// </summary>
	public static class RpcProcessor
	{
		private static AppHost _defaultServiceHost;
		private static readonly object InitilizeLock = new object();

		/// <summary>
		/// default ServiceHost
		/// </summary>
		public static AppHost ServiceHost => _defaultServiceHost;

		/// <summary>
		/// 
		/// </summary>
		/// <param name="config"></param>
		public static void Initialize(RpcLiteConfig config)
		{
			if (_defaultServiceHost != null)
			{
				return;
				//throw new InvalidOperationException("default service host already initialized");
			}

			lock (InitilizeLock)
			{
				if (_defaultServiceHost == null)
				{
					_defaultServiceHost = new AppHost(config);
					_defaultServiceHost.Initialize();
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
			return _defaultServiceHost.ProcessAsync(httpContext);
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="filter"></param>
		public static void AddFilter(IServiceFilter filter)
		{
			_defaultServiceHost.AddFilter(filter);
		}

	}
}