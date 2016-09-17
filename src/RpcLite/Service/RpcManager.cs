using System;
using System.Threading.Tasks;
using RpcLite.Config;

namespace RpcLite.Service
{
	/// <summary>
	/// 
	/// </summary>
	public static class RpcManager
	{
		private static readonly object InitilizeLock = new object();

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

			lock (InitilizeLock)
			{
				if (AppHost == null)
				{
					AppHost = new AppHost(config);
					AppHost.Initialize();
				}
			}
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="builder"></param>
		/// <returns></returns>
		public static void Initialize(Action<RpcConfigBuilder> builder)
		{
			var builderObj = new RpcConfigBuilder();
			builder(builderObj);
			var config = builderObj.Build();

			Initialize(config);
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="httpContext"></param>
		/// <returns>if true process processed or else the path not a service path</returns>
		public static Task<bool> ProcessAsync(IServerContext httpContext)
		{
			return AppHost.ProcessAsync(httpContext);
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="filter"></param>
		public static void AddFilter(IServiceFilter filter)
		{
			AppHost.AddFilter(filter);
		}

	}
}