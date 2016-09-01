using System;
using System.Collections.Generic;
using System.Linq;
using RpcLite.Config;

namespace RpcLite
{
	/// <summary>
	/// 
	/// </summary>
	public class AppHostBuilder
	{
		private readonly RpcLiteConfig _config = new RpcLiteConfig();

		/// <summary>
		/// 
		/// </summary>
		/// <param name="appId"></param>
		/// <returns></returns>
		public AppHostBuilder UseAppId(string appId)
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
		public AppHostBuilder UseRegistry(string name, Type factoryType, string address)
		{
			_config.Registry = new RegistryConfigItem(name, factoryType, address);
			return this;
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="name"></param>
		/// <param name="factoryType"></param>
		/// <param name="address"></param>
		/// <returns></returns>
		public AppHostBuilder UseMonitor(string name, Type factoryType, string address)
		{
			_config.Monitor = new MonitorConfigItem(name, factoryType, address);
			return this;
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="services"></param>
		/// <returns></returns>
		public AppHostBuilder UseServices(params ServiceConfigItem[] services)
		{
			if (services == null) return this;

			_config.Services = _config.Services ?? new List<ServiceConfigItem>();
			var newItems = services
				.Where(it => !_config.Services.Contains(it));
			_config.Services.AddRange(newItems);

			return this;
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="clients"></param>
		/// <returns></returns>
		public AppHostBuilder UseClients(params ClientConfigItem[] clients)
		{
			if (clients == null) return this;

			_config.Clients = _config.Clients ?? new List<Config.ClientConfigItem>();
			var newItems = clients
				.Where(it => !_config.Clients.Contains(it));
			_config.Clients.AddRange(newItems);

			return this;
		}

		/// <summary>
		/// 
		/// </summary>
		/// <returns></returns>
		public AppHost Build()
		{
			return new RpcLite.AppHost(_config);
		}

	}
}
