using System;
using System.Collections.Generic;
using System.Linq;
using RpcLite.Client;
using RpcLite.Monitor;
using RpcLite.Registry;
#if NETCORE
using System.Reflection;
#endif

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
		public RpcConfigBuilder UseRegistry(string name, Type factoryType, string address)
		{
			if (!typeof(IRegistryFactory).IsAssignableFromEx(factoryType))
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
			return UseRegistry(name, typeof(TFactory), address);
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
			if (!typeof(IMonitorFactory).IsAssignableFromEx(factoryType))
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
		public RpcConfigBuilder UseCluster<TFactory>(string name)
		{
			if (!typeof(IClusterFactory).IsAssignableFrom(typeof(TFactory)))
				throw new ArgumentOutOfRangeException(nameof(TFactory), $"typeof {nameof(TFactory)} must implements { nameof(IClusterFactory)}");

			if (_config.Client == null)
				_config.Client = new ClientConfig();

			_config.Client.Cluster = new ClusterConfig(name, typeof(TFactory));

			return this;
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="services"></param>
		/// <returns></returns>
		public RpcConfigBuilder UseServices(params ServiceConfigItem[] services)
		{
			if (services == null) return this;

			//_config.Services = _config.Services ?? new List<ServiceConfigItem>();
			//var newItems = services
			//	.Where(it => !_config.Services.Contains(it));
			//_config.Services.AddRange(newItems);

			_config.Service = _config.Service ?? new ServiceConfig();
			_config.Service.Services = _config.Service.Services ?? new List<ServiceConfigItem>();
			var newItems2 = services
				.Where(it => !_config.Service.Services.Contains(it));
			_config.Service.Services.AddRange(newItems2);

			return this;
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="name"></param>
		/// <param name="path"></param>
		/// <returns></returns>
		public RpcConfigBuilder UseService<TService>(string name, string path)
		{
			return UseService<TService>(name, path, null);
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="name"></param>
		/// <param name="path"></param>
		/// <param name="address"></param>
		/// <returns></returns>
		public RpcConfigBuilder UseService<TService>(string name, string path, string address)
		{
			var item = new ServiceConfigItem(name, typeof(TService), path)
			{
				Address = address
			};

			return UseServices(item);
		}

		/// <summary>
		/// <para>set service path prefix, eg: api/service/</para>
		/// <para> all match paths will be processed as service request</para>
		/// </summary>
		/// <param name="paths">must ends with "/", eg: api/service/</param>
		/// <returns></returns>
		public RpcConfigBuilder UseServicePaths(params string[] paths)
		{
			if (_config.Service == null)
				_config.Service = new ServiceConfig();

			if (paths != null)
			{
#if NETCORE
				if (!paths.All(it => it.EndsWith("/")))
					throw new ArgumentOutOfRangeException(nameof(paths), "all path must ends with /");

				paths = paths
					.Select(it => it + "{*RpcLiteServicePath}")
					//.Select(it => it + "{*path}")
					.ToArray();
#endif
			}

			_config.Service.Paths = paths;

			return this;
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="clients"></param>
		/// <returns></returns>
		public RpcConfigBuilder UseClients(params ClientConfigItem[] clients)
		{
			if (clients == null) return this;

			//_config.Clients = _config.Clients ?? new List<ClientConfigItem>();
			//var newItems = clients
			//	.Where(it => !_config.Clients.Contains(it));
			//_config.Clients.AddRange(newItems);

			_config.Client = _config.Client ?? new ClientConfig();
			_config.Client.Clients = _config.Client.Clients ?? new List<ClientConfigItem>();
			var newItems2 = clients
				.Where(it => !_config.Client.Clients.Contains(it));
			_config.Client.Clients.AddRange(newItems2);

			return this;
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="name"></param>
		/// <param name="address"></param>
		/// <returns></returns>
		public RpcConfigBuilder UseClient<TClient>(string name, string address)
		{
			var item = new ClientConfigItem(name, typeof(TClient), address)
			{
				Address = address
			};

			return UseClients(item);
		}

		/// <summary>
		/// 
		/// </summary>
		/// <returns></returns>
		public RpcConfig Build()
		{
			return _config;
		}

	}
}
