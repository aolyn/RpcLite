﻿using System;
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
		private readonly RegistryManager _registry;

		/// <summary>
		/// 
		/// </summary>
		/// <param name="registry"></param>
		public RpcClientBuilder(RegistryManager registry)
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
			//var uri = ClientAddressResolver<TContract>.GetAddress();
			var uri = _registry.GetAddress<TContract>();
			return uri == null ? null : uri.ToString();
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