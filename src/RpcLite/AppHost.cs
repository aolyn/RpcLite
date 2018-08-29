using System;
using System.Threading.Tasks;
using RpcLite.Client;
using RpcLite.Config;
using RpcLite.Filter;
using RpcLite.Monitor;
using RpcLite.Registry;
using RpcLite.Service;

#if NETCORE
using Microsoft.Extensions.DependencyInjection;
#endif

namespace RpcLite
{
	/// <summary>
	/// 
	/// </summary>
	public class AppHost
	{
		/// <summary>
		/// ServiceHost
		/// </summary>
		public ServiceHost ServiceHost { get; }

		/// <summary>
		/// ClientFactory
		/// </summary>
		public RpcClientFactory ClientFactory { get; }

		/// <summary>
		/// 
		/// </summary>
		public IMonitor Monitor { get; }

		/// <summary>
		/// 
		/// </summary>
		public IRegistry Registry { get; }

		/// <summary>
		/// 
		/// </summary>
		internal FormatterManager FormatterManager { get; set; }

		/// <summary>
		/// 
		/// </summary>
		internal VersionedList<IServiceFilter> ServiceFilters { get; } = new VersionedList<IServiceFilter>();

		/// <summary>
		/// 
		/// </summary>
		internal VersionedList<IRpcClientFilter> ClientFilters { get; } = new VersionedList<IRpcClientFilter>();

#if NETCORE
		internal bool SupportDi { get; }
#endif

		/// <summary>
		/// 
		/// </summary>
		public string AppId { get; }

#if NETCORE
		/// <summary>
		/// 
		/// </summary>
		public AppHost(RpcConfig config, IServiceCollection services) : this(config)
		{
			if (services != null)
			{
				SupportDi = true;

				if (config.Service?.Services != null)
				{
					foreach (var service in config.Service.Services)
					{
						var type = ReflectHelper.GetTypeByIdentifier(service.Type);
						switch (service.LifeCycle)
						{
							case ServiceLifeCycle.Singleton:
								services.AddSingleton(type);
								break;
							case ServiceLifeCycle.Scoped:
								services.AddScoped(type);
								break;
							case ServiceLifeCycle.Transient:
								services.AddTransient(type);
								break;
						}
					}
				}
			}
		}
#endif

		/// <summary>
		/// 
		/// </summary>
		public AppHost(RpcConfig config)
		{
			if (config == null)
				throw new ArgumentNullException(nameof(config));

			AppId = config.AppId;
			Registry = RegistryHelper.GetRegistry(config);
			ServiceHost = new ServiceHost(this, config.Service);
			FormatterManager = new FormatterManager(config.Formatter);
			ClientFactory = new RpcClientFactory(this, config.Client);

			if (config.Monitor != null && !string.IsNullOrWhiteSpace(config.Monitor.Type))
			{
				var monitorFactory = ReflectHelper.CreateInstanceByIdentifier<IMonitorFactory>(config.Monitor.Type);
				Monitor = monitorFactory.CreateMonitor(this, config);
			}

			if (config.Filter?.Filters.Count > 0)
			{
				foreach (var item in config.Filter.Filters)
				{
					var factory = ReflectHelper.CreateInstanceByIdentifier<IFilterFactory>(item.Type);
					var filters = factory.CreateFilters();
					if (filters == null) continue;
					foreach (var filter in filters)
					{
						AddFilter(filter);
					}
				}
			}
		}

		/// <summary>
		/// initialize service host
		/// </summary>
		public void Initialize()
		{
			ServiceHost.Initialize();
			//RegisterServices();
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="filter"></param>
		private void AddFilter(IRpcFilter filter)
		{
			switch (filter)
			{
				case null:
					throw new ArgumentNullException(nameof(filter));
				case IServiceFilter _:
					ServiceFilters.Add((IServiceFilter)filter);
					break;
				case IRpcClientFilter _:
					ClientFilters.Add((IRpcClientFilter)filter);
					break;
			}
		}

		/// <summary>
		/// process service request
		/// </summary>
		/// <param name="serverContext"></param>
		/// <returns>if true process processed or else the path not a service path</returns>
		public Task<bool> ProcessAsync(IServerContext serverContext)
		{
			return ServiceHost.ProcessAsync(serverContext);
		}

	}
}