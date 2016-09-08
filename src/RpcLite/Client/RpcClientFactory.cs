using System;
using System.Collections.Concurrent;
using RpcLite.Config;

namespace RpcLite.Client
{

	/// <summary>
	/// 
	/// </summary>
	public class RpcClientFactory
	{
		private readonly ConcurrentDictionary<Type, object> _clienBuilders = new ConcurrentDictionary<Type, object>();
		private readonly IClusterFactory _clusterFactory;
		//private readonly AppHost _appHost;
		//private readonly IClientChannelFactory _channelFactory;

		/// <summary>
		/// 
		/// </summary>
		/// <param name="appHost"></param>
		/// <param name="config"></param>
		public RpcClientFactory(AppHost appHost, RpcConfig config)
		{
			//_appHost = appHost;
			if (config?.Client != null)
			{
				//get ClusterFactory from config
			}

			_clusterFactory = config?.Client?.Cluster?.Type != null
				? TypeCreator.CreateInstanceByIdentifier<IClusterFactory>(config.Client?.Cluster?.Type)
				: new SimpleClusterFactory();
			//todo: create DefaultClientChannelFactory from config
			_clusterFactory.Initilize(appHost?.Registry, new DefaultClientChannelFactory());
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
			client.Cluster = string.IsNullOrWhiteSpace(url)
				? _clusterFactory.GetCluster<TContract>()
				: _clusterFactory.GetCluster<TContract>(url);
			return client as TContract;
		}

	}
}
