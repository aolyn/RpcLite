using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using RpcLite.Client;
using RpcLite.Config;
using RpcLite.Logging;
using ServiceRegistry.Contract;

namespace RpcLite.Registry.Http
{

	public class HttpRegistry : RegistryBase
	{
		private static readonly object InitializeLocker = new object();
		private static QuickReadConcurrentDictionary<ServiceIdentifier, ServiceInfo[]> _defaultBaseUrlDictionary = new QuickReadConcurrentDictionary<ServiceIdentifier, ServiceInfo[]>();
		private readonly Lazy<IRegistryService> _registryClient;
		private readonly AppHost _appHost;

		public HttpRegistry(AppHost appHost, RpcConfig config)
			: base(config)
		{
			Config = config;
			_appHost = appHost;

			_registryClient = new Lazy<IRegistryService>(() =>
			{
				var address = Config?.Registry?.Address;

				if (string.IsNullOrWhiteSpace(address))
				{
					LogHelper.Error("Registry Client Config Error: not exist or path is empty");
					return null;
				}

				var client = _appHost == null
					? ClientFactory.GetInstance<IRegistryService>(address)
					: _appHost.ClientFactory.GetInstance<IRegistryService>(address);
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
			var tempDic = GetAddresses<Dictionary<ServiceIdentifier, ServiceInfo[]>>(Config)
				.Where(it => it.Value?.Select(v => v.Address).FirstOrDefault() != null)
				.ToDictionary(it => it.Key, it => it.Value);

			var tmp = new QuickReadConcurrentDictionary<ServiceIdentifier, ServiceInfo[]>();
			foreach (var item in tempDic)
			{
				tmp.Add(item.Key, item.Value);
			}

			_defaultBaseUrlDictionary = tmp;
		}

		public override bool CanRegister => false;

		private ServiceInfo[] GetAddressInternal(string name, string group)
		{
			var itemKey = new ServiceIdentifier(name, group);
			// ReSharper disable once InconsistentlySynchronizedField
			var url = _defaultBaseUrlDictionary.GetOrAdd(itemKey, key =>
			{
				if (key == ServiceIdentifier.Empty) return null;
				if (_registryClient.Value == null) return null;

				var request = new GetServiceAddressRequest
				{
					ServiceName = name,
					Group = group,
				};
				try
				{
					var response = _registryClient.Value.GetServiceAddress(request);
					var uri = string.IsNullOrWhiteSpace(response?.Address)
						? null
						: response.Address;

					return new[]
					{
					new ServiceInfo
					{
						Name = name,
						Address = uri,
						Group = group,
					}
				};
				}
				catch (Exception ex)
				{
					LogHelper.Error(ex);
					throw;
				}
			});

			return url;
		}

		public override Task RegisterAsync(ServiceInfo serviceInfo)
		{
			throw new NotImplementedException();
		}

		public override Task<ServiceInfo[]> LookupAsync(string name, string group)
		{
#if NETCORE
			return Task.FromResult(GetAddressInternal(name, group));
#else
			return TaskHelper.FromResult(GetAddressInternal(name, group));
#endif
		}
	}

}
