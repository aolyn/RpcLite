using System;
using RpcLite.Config;

namespace RpcLite
{
	/// <summary>
	/// 
	/// </summary>
	public class AppHostBuilder
	{
		private readonly RpcConfigBuilder _config = new RpcConfigBuilder();

		/// <summary>
		/// 
		/// </summary>
		/// <param name="appId"></param>
		/// <returns></returns>
		public AppHostBuilder UseAppId(string appId)
		{
			_config.UseAppId(appId);
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
			_config.UseRegistry(name, factoryType, address);
			return this;
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="name"></param>
		/// <param name="address"></param>
		/// <returns></returns>
		public AppHostBuilder UseRegistry<TFactory>(string name, string address)
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
		public AppHostBuilder UseMonitor(string name, Type factoryType, string address)
		{
			_config.UseMonitor(name, factoryType, address);
			return this;
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="name"></param>
		/// <param name="address"></param>
		/// <returns></returns>
		public AppHostBuilder UseMonitor<TFactory>(string name, string address)
		{
			return UseMonitor(name, typeof(TFactory), address);
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="name"></param>
		/// <returns></returns>
		public AppHostBuilder UseServiceMapper<TFactory>(string name)
		{
			_config.UseServiceMapper<TFactory>(name);
			return this;
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="services"></param>
		/// <returns></returns>
		public AppHostBuilder UseServices(params ServiceConfigItem[] services)
		{
			_config.UseServices(services);
			return this;
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="name"></param>
		/// <param name="path"></param>
		/// <param name="address"></param>
		/// <returns></returns>
		public AppHostBuilder UseService<TService>(string name, string path, string address)
		{
			_config.UseService<TService>(name, path, address);
			return this;
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="name"></param>
		/// <param name="path"></param>
		/// <returns></returns>
		public AppHostBuilder UseService<TService>(string name, string path)
		{
			return UseService<TService>(name, path, null);
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="paths"></param>
		/// <returns></returns>
		public AppHostBuilder UseServicePaths(params string[] paths)
		{
			_config.UseServicePaths(paths);
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
			_config.UseClients(clients);
			return this;
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="name"></param>
		/// <param name="address"></param>
		/// <returns></returns>
		public AppHostBuilder UseClient<TClient>(string name, string address)
		{
			_config.UseClient<TClient>(name, address);
			return this;
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="name"></param>
		/// <returns></returns>
		public AppHostBuilder UseInvoker<TFactory>(string name)
		{
			_config.UseInvoker<TFactory>(name);

			return this;
		}

		/// <summary>
		/// 
		/// </summary>
		/// <typeparam name="TFactory"></typeparam>
		/// <returns></returns>
		public AppHostBuilder UseChannelProvider<TFactory>()
		{
			return UseChannelProvider<TFactory>(null);
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="name"></param>
		/// <returns></returns>
		public AppHostBuilder UseChannelProvider<TFactory>(string name)
		{
			_config.UseChannelProvider<TFactory>(name);

			return this;
		}


		/// <summary>
		/// 
		/// </summary>
		/// <returns></returns>
		public AppHostBuilder UseFilter<TFactory>()
		{
			UseFilter<TFactory>(null);
			return this;
		}

		/// <summary>
		/// 
		/// </summary>
		/// <returns></returns>
		public AppHostBuilder UseFilter<TFactory>(string name)
		{
			_config.UseFilter<TFactory>(name);
			return this;
		}

		/// <summary>
		/// 
		/// </summary>
		/// <typeparam name="TFormatter"></typeparam>
		/// <returns></returns>
		public AppHostBuilder UseFormatter<TFormatter>()
		{
			_config.UseFormatter<TFormatter>(null);
			return this;
		}

		/// <summary>
		/// 
		/// </summary>
		/// <returns></returns>
		public AppHostBuilder UseFormatter<TFormatter>(string name)
		{
			_config.UseFormatter<TFormatter>(name);
			return this;
		}


		/// <summary>
		/// 
		/// </summary>
		/// <returns></returns>
		public AppHost Build()
		{
			return new AppHost(_config.Build());
		}

	}
}
