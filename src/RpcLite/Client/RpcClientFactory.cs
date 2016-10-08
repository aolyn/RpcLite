using System;
using System.Collections.Concurrent;
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

			_invokerFactory = config?.Client?.Invoker?.Type != null
				? TypeCreator.CreateInstanceByIdentifier<IInvokerFactory>(config.Client?.Invoker?.Type)
				: new DefaultInvokerFactory();
			//todo: create DefaultClientChannelFactory from config
			_invokerFactory.Initilize(appHost?.Registry, new DefaultClientChannelFactory());
		}

		/// <summary>
		/// 
		/// </summary>
		/// <typeparam name="TContract"></typeparam>
		/// <returns></returns>
		public TContract GetInstance<TContract>()
			where TContract : class
		{
			return GetInstance<TContract>(null);
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
		public TContract GetInstance<TContract>(string url)
			where TContract : class
		{
			var builder = GetBuilder<TContract>();
			var client = builder.GetInstance(url);
			client.Invoker = string.IsNullOrWhiteSpace(url)
				? _invokerFactory.GetInvoker<TContract>()
				: _invokerFactory.GetInvoker<TContract>(url);
			client.Formatter = _appHost?.FormatterManager?.DefaultFormatter
				?? FormatterManager.Default.DefaultFormatter;
			client.AppHost = _appHost;
			return client as TContract;
		}

	}
}
