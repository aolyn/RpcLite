using System;
using System.Collections.Concurrent;
using RpcLite.Registry;

namespace RpcLite.Client
{

	/// <summary>
	/// 
	/// </summary>
	public class RpcClientFactory
	{
		private readonly ConcurrentDictionary<Type, object> _clienBuilders = new ConcurrentDictionary<Type, object>();
		private readonly IRegistry _registry;

		/// <summary>
		/// 
		/// </summary>
		/// <param name="registry"></param>
		public RpcClientFactory(IRegistry registry)
		{
			_registry = registry;
		}

		/// <summary>
		/// 
		/// </summary>
		/// <typeparam name="TContract"></typeparam>
		/// <returns></returns>
		public TContract GetInstance<TContract>()
			where TContract : class
		{
			var builder = GetBuilder<TContract>();
			return builder.GetInstance();
		}

		private RpcClientBuilder<TContract> GetBuilder<TContract>() where TContract : class
		{
			var type = typeof(TContract);
			var builder = (RpcClientBuilder<TContract>)_clienBuilders.GetOrAdd(type, tp =>
			{
				var b = new RpcClientBuilder<TContract>(_registry);
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
			return builder.GetInstance(url);
		}
	}
}
