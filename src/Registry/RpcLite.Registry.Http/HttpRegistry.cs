﻿using System;
using System.Linq;
using System.Threading.Tasks;
using RpcLite.Client;
using RpcLite.Config;
using RpcLite.Logging;
using ServiceRegistry.Contract;

namespace RpcLite.Registry.Http
{
	public class HttpRegistry : IRegistry
	{
		private static readonly object InitializeLocker = new object();
		private static QuickReadConcurrentDictionary<ClientConfigItem, string> _defaultBaseUrlDictionary = new QuickReadConcurrentDictionary<ClientConfigItem, string>();
		private readonly RpcConfig _config;
		private readonly Lazy<IRegistryService> _registryClient;

		public HttpRegistry(RpcConfig config)
		{
			_config = config;

			_registryClient = new Lazy<IRegistryService>(() =>
			{
				var address = _config?.Registry?.Address;

				//var registryClientConfigItem = _config.Clients
				//	.FirstOrDefault(it => it.TypeName == typeof(IRegistryService).FullName);

				if (string.IsNullOrWhiteSpace(address))
				{
					LogHelper.Error("Registry Client Config Error: not exist or path is empty");
					return null;
				}

				var client = ClientFactory.GetInstance<IRegistryService>(address);
				return client;
			});

			InitilizeBaseUrlsSafe();
		}

		private void InitilizeBaseUrlsSafe()
		{
			lock (InitializeLocker)
			{
				InitilizeBaseUrls();
			}
		}

		private void InitilizeBaseUrls()
		{
			var tempDic = new QuickReadConcurrentDictionary<ClientConfigItem, string>();

			var clients = _config.Client?.Clients;
			if (clients != null)
			{
				foreach (var item in clients)
				{
					if (!string.IsNullOrWhiteSpace(item.Address))
						tempDic.Add(item, item.Address);
				}
			}

			_defaultBaseUrlDictionary = tempDic;
		}

		public bool CanRegister => false;

		private string[] GetAddressInternal(ClientConfigItem clientInfo)
		{
			// ReSharper disable once InconsistentlySynchronizedField
			var url = _defaultBaseUrlDictionary.GetOrAdd(clientInfo, key =>
			{
				var clientConfigItem = clientInfo;

				if (clientConfigItem == null) return null;
				if (_registryClient.Value == null) return null;

				var request = new GetServiceAddressRequest
				{
					ServiceName = clientConfigItem.Name,
					Namespace = clientConfigItem.Namespace,
					Environment = clientInfo.Environment,
				};
				var response = _registryClient.Value.GetServiceAddress(request);
				var uri = string.IsNullOrWhiteSpace(response?.Address)
					? null
					: response.Address;

				return uri;
			});

			return string.IsNullOrEmpty(url)
				? null
				: new[] { (url) };
		}

		public void OnConfigChanged()
		{
			InitilizeBaseUrlsSafe();
		}

		public Task RegisterAsync(ServiceConfigItem serviceInfo)
		{
			throw new NotImplementedException();
		}

		public Task<string[]> LookupAsync(ClientConfigItem clientInfo)
		{
#if NETCORE
			return Task.FromResult(GetAddressInternal(clientInfo));
#else
			return TaskHelper.FromResult(GetAddressInternal(clientInfo));
#endif
		}

		public Task<string[]> LookupAsync<TContract>()
		{
			var type = typeof(TContract);
			var clientConfigItem = _config.Client.Clients
				.FirstOrDefault(it => it.TypeName == type.FullName);

			return clientConfigItem == null
				? TaskHelper.FromResult<string[]>(null)
				: LookupAsync(clientConfigItem);
		}

		public void Dispose()
		{

		}

	}
}
