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
		private readonly RpcConfig _config;
		//private readonly Lazy<object> _initializeRegistry;

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
		//public IClusterFactory ClusterFactory { get; }

		/// <summary>
		/// 
		/// </summary>
		public VersionedList<IServiceFilter> Filters { get; internal set; } = new VersionedList<IServiceFilter>();

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

			_config = config;

			AppId = config.AppId;
			Registry = RegistryHelper.GetRegistry(config);
			ServiceHost = new ServiceHost(this, config);
			//ClusterFactory = new SimpleClusterFactory();
			ClientFactory = new RpcClientFactory(this, config);

			if (!string.IsNullOrWhiteSpace(config.Monitor?.Type))
			{
				var monitorFactory = TypeCreator.CreateInstanceByIdentifier<IMonitorFactory>(config.Monitor.Type);
				Monitor = monitorFactory.CreateMonitor(config);
			}

			//_initializeRegistry = new Lazy<object>(() =>
			//{
			//	RegistryServices();

			//	return null;
			//});
		}

		private void RegisterServices()
		{
			if (Registry?.CanRegister != true || _config?.Service.Services == null) return;

			foreach (var service in _config.Service.Services)
			{
				Registry.RegisterAsync(service);
			}
		}

		/// <summary>
		/// initialize service host
		/// </summary>
		public void Initialize()
		{
			ServiceHost.Initialize();

			// ReSharper disable once UnusedVariable
			//var initilizeResult = _initializeRegistry.Value;
			RegisterServices();
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="filter"></param>
		public void AddFilter(IServiceFilter filter)
		{
			Filters.Add(filter);
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="filter"></param>
		public void RemoveFilter(IServiceFilter filter)
		{
			Filters.Remove(filter);
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="serverContext"></param>
		/// <returns>if true process processed or else the path not a service path</returns>
		public Task<bool> ProcessAsync(IServerContext serverContext)
		{
			return ServiceHost.ProcessAsync(serverContext);
		}

	}
}