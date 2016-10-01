using System;
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
		private static QuickReadConcurrentDictionary<ClientConfigItem, string> _defaultBaseUrlDictionary = new QuickReadConcurrentDictionary<ClientConfigItem, string>();
		private readonly Lazy<IRegistryService> _registryClient;

		public HttpRegistry(RpcConfig config)
			: base(config)
		{
			Config = config;

			_registryClient = new Lazy<IRegistryService>(() =>
			{
				var address = Config?.Registry?.Address;

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
			var tempDic = GetAddresses<QuickReadConcurrentDictionary<ClientConfigItem, string>>(Config);

			_defaultBaseUrlDictionary = tempDic;
		}

		public override bool CanRegister => false;

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
					//Namespace = clientConfigItem.Namespace,
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

		public override Task RegisterAsync(ServiceConfigItem serviceInfo)
		{
			throw new NotImplementedException();
		}

		public override Task<string[]> LookupAsync(ClientConfigItem clientInfo)
		{
#if NETCORE
			return Task.FromResult(GetAddressInternal(clientInfo));
#else
			return TaskHelper.FromResult(GetAddressInternal(clientInfo));
#endif
		}
	}

}
