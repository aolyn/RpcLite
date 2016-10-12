using System;
using System.Collections.Concurrent;
using System.Linq;
using RpcLite.Config;
using RpcLite.Service;

namespace RpcLite.Client
{

	/// <summary>
	/// 
	/// </summary>
	public class RpcClientFactory
	{
		private readonly ConcurrentDictionary<Type, object> _clienBuilders = new ConcurrentDictionary<Type, object>();
		private readonly IInvokerFactory _invokerFactory;
		private readonly AppHost _appHost;
		private readonly RpcConfig _config;

		//private readonly IClientChannelFactory _channelFactory;

		/// <summary>
		/// 
		/// </summary>
		/// <param name="appHost"></param>
		/// <param name="config"></param>
		public RpcClientFactory(AppHost appHost, RpcConfig config)
		{
			_appHost = appHost;
			if (config?.Client != null)
			{
				//get InvokerFactory from config
			}

			_config = config;

			var channelFactory = new DefaultChannelFactory();
			channelFactory.Initialize(config);

			_invokerFactory = config?.Client?.Invoker?.Type != null
				? TypeCreator.CreateInstanceByIdentifier<IInvokerFactory>(config.Client?.Invoker?.Type)
				: new DefaultInvokerFactory();
			_invokerFactory.Initilize(appHost?.Registry, channelFactory);
		}

		private RpcClientBuilder<TContract> GetBuilder<TContract>() where TContract : class
		{
			var type = typeof(TContract);
			var builder = (RpcClientBuilder<TContract>)_clienBuilders.GetOrAdd(type, tp =>
			{
				var b = new RpcClientBuilder<TContract>();
				return b;
			});
			return builder;
		}

		/// <summary>
		/// 
		/// </summary>
		/// <typeparam name="TContract"></typeparam>
		/// <returns></returns>
		public TContract GetInstance<TContract>()
			where TContract : class
		{
			var type = typeof(TContract);
			var clientConfigItem = _config?.Client?.Clients
				.FirstOrDefault(it => it.TypeName == type.FullName);

			return GetInstance<TContract>(clientConfigItem?.Name, clientConfigItem?.Group, null);
		}

		/// <summary>
		/// 
		/// </summary>
		/// <typeparam name="TContract"></typeparam>
		/// <returns></returns>
		public TContract GetInstance<TContract>(string url)
			where TContract : class
		{
			return GetInstance<TContract>(null, null, url);
		}


		/// <summary>
		/// 
		/// </summary>
		/// <typeparam name="TContract"></typeparam>
		/// <param name="name"></param>
		/// <param name="group"></param>
		/// <returns></returns>
		public TContract GetInstance<TContract>(string name, string group)
			where TContract : class
		{
			return GetInstance<TContract>(name, group, null);
		}

		/// <summary>
		/// 
		/// </summary>
		/// <typeparam name="TContract"></typeparam>
		/// <param name="name"></param>
		/// <param name="group"></param>
		/// <param name="address"></param>
		/// <returns></returns>
		private TContract GetInstance<TContract>(string name, string group, string address)
			where TContract : class
		{
			var builder = GetBuilder<TContract>();
			var client = builder.GetInstance(address);
			client.Name = name;
			client.Group = group;
			var invoker = string.IsNullOrWhiteSpace(address)
				? _invokerFactory.GetInvoker<TContract>()
				: _invokerFactory.GetInvoker<TContract>(address);
			invoker.Initialize(name, group);
			client.Invoker = invoker;
			client.Formatter = _appHost?.FormatterManager?.DefaultFormatter
				?? FormatterManager.Default.DefaultFormatter;
			client.AppHost = _appHost;
			return client as TContract;
		}

	}
}
