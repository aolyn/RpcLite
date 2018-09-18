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
		private readonly ConcurrentDictionary<Type, object> _clientBuilders = new ConcurrentDictionary<Type, object>();
		private readonly IInvokerFactory _invokerFactory;
		private readonly AppHost _appHost;
		private readonly ClientConfig _clientConfig;

		//private readonly IClientChannelFactory _channelFactory;

		/// <summary>
		/// 
		/// </summary>
		/// <param name="appHost"></param>
		/// <param name="config"></param>
		public RpcClientFactory(AppHost appHost, ClientConfig config)
		{
			_appHost = appHost;
			if (config != null)
			{
				//get InvokerFactory from config
			}

			_clientConfig = config;

			var channelFactory = new DefaultChannelFactory();
			channelFactory.Initialize(config);

			_invokerFactory = config?.Invoker?.Type != null
				? ReflectHelper.CreateInstanceByIdentifier<IInvokerFactory>(config.Invoker?.Type)
				: new DefaultInvokerFactory();
			_invokerFactory.Initilize(appHost?.Registry, channelFactory);
		}

		private RpcClientBuilder GetBuilder(Type contractType)
		{
			var builder = (RpcClientBuilder)_clientBuilders.GetOrAdd(contractType, tp =>
			{
				var b = new RpcClientBuilder(contractType);
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
			var clientConfigItem = _clientConfig?.Clients
				.FirstOrDefault(it => it.TypeName == type.FullName);

			return GetInstance<TContract>(clientConfigItem?.Name, clientConfigItem?.Group, null);
		}

		/// <summary>
		/// 
		/// </summary>
		/// <returns></returns>
		internal RpcClientBase GetInstance(Type contractType)
		{
			var clientConfigItem = _clientConfig?.Clients
				.FirstOrDefault(it => it.TypeName == contractType.FullName);

			return GetInstance(clientConfigItem?.Name, clientConfigItem?.Group, null, contractType);
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
			var contractType = typeof(TContract);
			var client = GetInstance(name, group, address, contractType);
			return client as TContract;
		}

		private RpcClientBase GetInstance(string name, string group, string address, Type contractType)
		{
			var builder = GetBuilder(contractType);
			var client = builder.GetInstance(address);
			client.Name = name;
			client.Group = group;
			var invoker = string.IsNullOrWhiteSpace(address)
				? _invokerFactory.GetInvoker()
				: _invokerFactory.GetInvoker(address);
			invoker.Initialize(name, group);
			client.Invoker = invoker;
			client.Formatter = _appHost?.FormatterManager?.DefaultFormatter
				?? FormatterManager.Default.DefaultFormatter;
			client.AppHost = _appHost;
			return client;
		}
	}
}
