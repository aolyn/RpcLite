using System;
using System.Threading.Tasks;
using RpcLite.Config;
using RpcLite.Service;

#if NETCORE
using Microsoft.Extensions.DependencyInjection;
#endif

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

#if NETCORE
		/// <summary>
		/// 
		/// </summary>
		/// <param name="config"></param>
		/// <param name="services"></param>
		public static void Initialize(RpcConfig config, IServiceCollection services)
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
					AppHost = new AppHost(config, services);
					//AppHost.Initialize();
				}
			}
		}
#endif

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
		/// <param name="serverContext"></param>
		/// <returns>if true process processed or else the path not a service path</returns>
		public static Task<bool> ProcessAsync(IServerContext serverContext)
		{
			return AppHost.ProcessAsync(serverContext);
		}
	}
}