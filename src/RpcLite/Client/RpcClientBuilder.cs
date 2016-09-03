using System;
using System.Linq;
using RpcLite.Registry;

namespace RpcLite.Client
{
	/// <summary>
	/// 
	/// </summary>
	/// <typeparam name="TContract">contract interface</typeparam>
	public class RpcClientBuilder<TContract>
		where TContract : class
	{
		private readonly IRegistry _registry;

		/// <summary>
		/// 
		/// </summary>
		/// <param name="registry"></param>
		public RpcClientBuilder(IRegistry registry)
		{
			_registry = registry;
		}

		private static readonly Lazy<Func<RpcClientBase<TContract>>> ClientCreateFunc = new Lazy<Func<RpcClientBase<TContract>>>(() =>
		{
			var type = ClientWrapper.WrapInterface<TContract>();
			var func = TypeCreator.GetCreateInstanceFunc(type) as Func<RpcClientBase<TContract>>;
			return func;
		}, true);

		/// <summary>
		/// 
		/// </summary>
		/// <returns></returns>
		public TContract GetInstance()
		{
			return GetInstance(GetDefaultBaseUrl());
		}

		private string GetDefaultBaseUrl()
		{
			var uri = _registry.LookupAsync<TContract>().Result?.FirstOrDefault();
			return uri;
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="baseUrl"></param>
		/// <returns></returns>
		public TContract GetInstance(string baseUrl)
		{
			if (ClientCreateFunc.Value == null)
				throw new ClientException("GetCreateInstanceFunc Error.");

			var client = ClientCreateFunc.Value();
			client.Address = baseUrl;
			client.Channel = new HttpClientChannel(baseUrl);
			return client as TContract;
		}

	}
}
