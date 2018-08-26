using System;
using System.Threading.Tasks;
using RpcLite.Client;
using RpcLite.Config;
using RpcLite.Monitor;
using RpcLite.Registry;
using RpcLite.Service;

#if NETCORE
using CoreConfig = Microsoft.Extensions.Configuration;
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
		/// get config, modification to config will not effect except service.paths, so just don't modify it
		/// </summary>
		public RpcConfig Config { get; }

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

		/// <summary>
		/// 
		/// </summary>
		public string AppId { get; private set; }

#if NETCORE
		/// <summary>
		/// 
		/// </summary>
		public AppHost(CoreConfig.IConfiguration config)
			: this(new CoreConfiguration(config))
		{
		}
#endif

		/// <summary>
		/// 
		/// </summary>
		public AppHost(IConfiguration config)
			: this(RpcConfigHelper.GetConfig(config))
		{
		}

		/// <summary>
		/// 
		/// </summary>
		public AppHost(RpcConfig config)
		{
			if (config == null)
				throw new ArgumentNullException(nameof(config));

			Config = config;

			AppId = config.AppId;
			Registry = RegistryHelper.GetRegistry(this, config);
			ServiceHost = new ServiceHost(this, config);
			FormatterManager = new FormatterManager(config);
			ClientFactory = new RpcClientFactory(this, config);

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

		private void RegisterServices()
		{
			if (Registry?.CanRegister != true || Config?.Service.Services == null) return;

			foreach (var service in Config.Service.Services)
			{
				Registry.RegisterAsync(service.ToServiceInfo());
			}
		}

		/// <summary>
		/// initialize service host
		/// </summary>
		public void Initialize()
		{
			ServiceHost.Initialize();
			RegisterServices();
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