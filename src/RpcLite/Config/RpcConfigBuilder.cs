using System;
using System.Collections.Generic;
using System.Linq;
using RpcLite.Client;
using RpcLite.Filter;
using RpcLite.Formatters;
using RpcLite.Monitor;
using RpcLite.Registry;

namespace RpcLite.Config
{
	/// <summary>
	/// 
	/// </summary>
	public class RpcConfigBuilder
	{
		private readonly RpcConfig _config = new RpcConfig();

		/// <summary>
		/// 
		/// </summary>
		/// <param name="appId"></param>
		/// <returns></returns>
		public RpcConfigBuilder UseAppId(string appId)
		{
			_config.AppId = appId;
			return this;
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="name"></param>
		/// <param name="factoryType"></param>
		/// <param name="address"></param>
		/// <returns></returns>
		public RpcConfigBuilder UserRegistry(string name, Type factoryType, string address)
		{
			if (!typeof(IRegistryFactory).IsAssignableFrom(factoryType))
			{
				throw new ArgumentOutOfRangeException(nameof(factoryType), "factoryType must implement " + nameof(IRegistryFactory));
			}
			_config.Registry = new RegistryConfig(name, factoryType, address);
			return this;
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="name"></param>
		/// <param name="address"></param>
		/// <returns></returns>
		public RpcConfigBuilder UseRegistry<TFactory>(string name, string address)
		{
			return UserRegistry(name, typeof(TFactory), address);
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="name"></param>
		/// <param name="factoryType"></param>
		/// <param name="address"></param>
		/// <returns></returns>
		public RpcConfigBuilder UseMonitor(string name, Type factoryType, string address)
		{
			if (!typeof(IMonitorFactory).IsAssignableFrom(factoryType))
			{
				throw new ArgumentOutOfRangeException(nameof(factoryType), "factoryType must implement " + nameof(IMonitorFactory));
			}

			_config.Monitor = new MonitorConfig(name, factoryType, address);
			return this;
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="name"></param>
		/// <param name="address"></param>
		/// <returns></returns>
		public RpcConfigBuilder UseMonitor<TFactory>(string name, string address)
		{
			return UseMonitor(name, typeof(TFactory), address);
		}

		/// <summary>
		/// 
		/// </summary>
		/// <returns></returns>
		public RpcConfigBuilder UseMonitor<TFactory>()
		{
			return UseMonitor<TFactory>(null, null);
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="name"></param>
		/// <returns></returns>
		public RpcConfigBuilder UseServiceMapper<TFactory>(string name)
		{
			if (_config.Service == null)
				_config.Service = new ServiceConfig();

			_config.Service.Mapper = new ServiceMapperConfig(name, typeof(TFactory));

			return this;
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="name"></param>
		/// <returns></returns>
		public RpcConfigBuilder UseInvoker<TFactory>(string name)
		{
			if (!typeof(IInvokerFactory).IsAssignableFrom(typeof(TFactory)))
				throw new ArgumentOutOfRangeException(nameof(TFactory), $"typeof {nameof(TFactory)} must implements { nameof(IInvokerFactory)}");

			if (_config.Client == null)
				_config.Client = new ClientConfig();

			_config.Client.Invoker = new InvokerConfig(name, typeof(TFactory));

			return this;
		}

		#region AddService

		/// <summary>
		/// 
		/// </summary>
		/// <param name="services"></param>
		/// <returns></returns>
		public RpcConfigBuilder AddServices(params ServiceConfigItem[] services)
		{
			if (services == null) return this;

			_config.Service = _config.Service ?? new ServiceConfig();
			_config.Service.Services = _config.Service.Services ?? new List<ServiceConfigItem>();

			//check duplicate
			var newNames = services.Select(it => it.Name);
			var oldNames = _config.Service.Services.Select(it => it.Name);
			var existNames = newNames.Intersect(oldNames).ToArray();
			if (existNames.Length > 0)
			{
				throw new RpcConfigException("service already exist: " + string.Join(",", existNames));
			}

			_config.Service.Services.AddRange(services);

			return this;
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="name"></param>
		/// <param name="path"></param>
		/// <param name="address"></param>
		/// <param name="lifeCycle"></param>
		/// <returns></returns>
		public RpcConfigBuilder AddService<TService>(string name, string path, string address,
			ServiceLifecycle lifeCycle)
		{
			var item = new ServiceConfigItem(name, typeof(TService), path)
			{
				Address = address,
				LifeCycle = lifeCycle,
			};

			return AddServices(item);
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="name"></param>
		/// <param name="path"></param>
		/// <param name="address"></param>
		/// <returns></returns>
		public RpcConfigBuilder AddService<TService>(string name, string path, string address)
		{
			return AddService<TService>(name, path, address, ServiceLifecycle.Singleton);
		}

		/// <summary>
		/// add a service with name as Type name, eg: UseService&lt;TestService&gt;("api/test/")
		/// </summary>
		/// <param name="name"></param>
		/// <param name="path">url path, must ends with /</param>
		/// <returns></returns>
		public RpcConfigBuilder AddService<TService>(string name, string path)
		{
			if (path == null)
				throw new ArgumentNullException(nameof(path));

			if (!path.EndsWith("/"))
				throw new ArgumentOutOfRangeException($"{nameof(path)} must ends with /");

			return AddService<TService>(name, path, null);
		}

		/// <summary>
		/// add a service use type name as service name,
		/// <para>eg: UseService&lt;TestService&gt;("api/test/") service name is typeof(TService).Name</para>
		/// 
		/// </summary>
		/// <param name="path">url path, must ends with /</param>
		/// <returns></returns>
		public RpcConfigBuilder AddService<TService>(string path)
		{
			return AddService<TService>(typeof(TService).Name, path);
		}

		#endregion

		#region AddClient

		/// <summary>
		/// 
		/// </summary>
		/// <param name="clients"></param>
		/// <returns></returns>
		public RpcConfigBuilder AddClients(params ClientConfigItem[] clients)
		{
			if (clients == null) return this;

			_config.Client = _config.Client ?? new ClientConfig();
			_config.Client.Clients = _config.Client.Clients ?? new List<ClientConfigItem>();

			//check duplicate
			var newNames = clients.Select(it => it.Name);
			var oldNames = _config.Client.Clients.Select(it => it.Name);
			var existNames = newNames.Intersect(oldNames).ToArray();
			if (existNames.Length > 0)
			{
				throw new RpcConfigException("service already exist: " + string.Join(",", existNames));
			}

			_config.Client.Clients.AddRange(clients);

			return this;
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="name">name of Service<para>if Registry will be use, Service Name must be set</para></param>
		/// <param name="group"></param>
		/// <param name="address"></param>
		/// <returns></returns>
		public RpcConfigBuilder AddClient<TClient>(string name, string group, string address)
		{
			var item = new ClientConfigItem(name, typeof(TClient), address)
			{
				Group = group,
				Address = address
			};

			return AddClients(item);
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="name">name of Service<para>if Registry will be use, Service Name must be set</para></param>
		/// <param name="address"></param>
		/// <returns></returns>
		public RpcConfigBuilder AddClient<TClient>(string name, string address)
		{
			return AddClient<TClient>(name, null, address);
		}

		/// <summary>
		/// if Registry will be use, Service Name must be set, use UseClient&lt;TClient&gt;(string name, string address)
		/// </summary>
		/// <returns></returns>
		public RpcConfigBuilder AddClient<TClient>(string name)
		{
			return AddClient<TClient>(name, null);
		}

		#endregion

		#region AddFilter

		/// <summary>
		/// 
		/// </summary>
		/// <returns></returns>
		public RpcConfigBuilder AddFilter<TFactory>()
		{
			AddFilter<TFactory>(null);
			return this;
		}

		/// <summary>
		/// 
		/// </summary>
		/// <returns></returns>
		public RpcConfigBuilder AddFilter<TFactory>(string name)
		{
			if (_config.Filter == null)
			{
				_config.Filter = new FilterConfig
				{
					Filters = new List<FilterItemConfig>()
				};
			}

			if (!typeof(IFilterFactory).IsAssignableFrom(typeof(TFactory)))
			{
				throw new ArgumentOutOfRangeException(nameof(TFactory), "factoryType must implement " + nameof(IFilterFactory));
			}

			_config.Filter.Filters.Add(new FilterItemConfig(name, typeof(TFactory)));

			return this;
		}

		#endregion

		#region AddFormatter

		/// <summary>
		/// 
		/// </summary>
		/// <returns></returns>
		public RpcConfigBuilder AddFormatter<TFormatter>()
		{
			AddFormatter<TFormatter>(null);
			return this;
		}

		/// <summary>
		/// 
		/// </summary>
		/// <returns></returns>
		public RpcConfigBuilder AddFormatter<TFormatter>(string name)
		{
			if (_config.Formatter == null)
				_config.Formatter = new FormatterConfig();
			if (_config.Formatter.Formatters == null)
				_config.Formatter.Formatters = new List<FormatterItemConfig>();

			if (!typeof(IFormatter).IsAssignableFrom(typeof(TFormatter)))
			{
				throw new ArgumentOutOfRangeException(nameof(TFormatter), "factoryType must implement " + nameof(IFormatter));
			}

			_config.Formatter.Formatters.Add(new FormatterItemConfig(name, typeof(TFormatter)));

			return this;
		}

		#endregion

		/// <summary>
		/// 
		/// </summary>
		/// <typeparam name="TFactory"></typeparam>
		/// <param name="name"></param>
		public RpcConfigBuilder UseChannelProvider<TFactory>(string name)
		{
			if (!typeof(IChannelProvider).IsAssignableFrom(typeof(TFactory)))
				throw new ArgumentOutOfRangeException(nameof(TFactory),
					"factoryType must implement " + nameof(IChannelProvider));

			if (_config.Client == null)
				_config.Client = new ClientConfig();
			if (_config.Client.Channel == null)
				_config.Client.Channel = new ChannelConfig();
			if (_config.Client.Channel.Providers == null)
				_config.Client.Channel.Providers = new List<ChannelProviderConfig>();

			_config.Client.Channel.Providers.Add(new ChannelProviderConfig(name, typeof(TFactory)));
			return this;
		}

		/// <summary>
		/// 
		/// </summary>
		/// <returns></returns>
		public RpcConfig Build()
		{
			return _config;
		}

		/// <summary>
		/// build config by call builder(new RpcConfigBuilder()), if builder is null return new RpcConfig()
		/// </summary>
		/// <param name="builder"></param>
		/// <returns></returns>
		public static RpcConfig BuildConfig(Action<RpcConfigBuilder> builder)
		{
			if (builder == null)
			{
				return new RpcConfig();
			}

			var builderObj = new RpcConfigBuilder();
			builder(builderObj);
			var config = builderObj.Build();
			return config;
		}

	}
}
