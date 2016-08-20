using System;
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

		static HttpRegistry()
		{
			InitilizeBaseUrlsSafe();
		}

		private static void InitilizeBaseUrlsSafe()
		{
			lock (InitializeLocker)
			{
				InitilizeBaseUrls();
			}
		}

		private static void InitilizeBaseUrls()
		{
			var tempDic = new QuickReadConcurrentDictionary<ClientConfigItem, string>();
			foreach (var item in RpcLiteConfig.Instance.Clients)
			{
				if (!string.IsNullOrWhiteSpace(item.Path))
					tempDic.Add(item, item.Path);
			}

			_defaultBaseUrlDictionary = tempDic;
		}

		private static readonly Lazy<IRegistryService> RegistryClient =
			new Lazy<IRegistryService>(() =>
			{
				var registryClientConfigItem = RpcLiteConfig.Instance.Clients
					.FirstOrDefault(it => it.TypeName == typeof(IRegistryService).FullName);

				if (string.IsNullOrWhiteSpace(registryClientConfigItem?.Path))
				{
					LogHelper.Error("Registry Client Config Error: not exist or path is empty");
					return null;
				}

				var client = ClientFactory.GetInstance<IRegistryService>(registryClientConfigItem.Path);
				return client;
			});

		public bool CanRegister => false;

		private static Uri[] GetAddressInternal(ClientConfigItem clientInfo)
		{
			// ReSharper disable once InconsistentlySynchronizedField
			var url = _defaultBaseUrlDictionary.GetOrAdd(clientInfo, () =>
			{
				var clientConfigItem = clientInfo;

				if (clientConfigItem == null) return null;
				if (RegistryClient.Value == null) return null;

				var request = new GetServiceAddressRequest
				{
					ServiceName = clientConfigItem.Name,
					Namespace = clientConfigItem.Namespace,
					Environment = clientInfo.Environment,
				};
				var response = RegistryClient.Value.GetServiceAddress(request);
				var uri = string.IsNullOrWhiteSpace(response?.Address)
					? null
					: response.Address;

				return uri;
			});

			return string.IsNullOrEmpty(url)
				? null
				: new[] { new Uri(url) };
		}

		public void OnConfigChanged()
		{
			InitilizeBaseUrlsSafe();
		}

		public Task RegisterAsync(ServiceConfigItem serviceInfo)
		{
			throw new NotImplementedException();
		}

		public Task<Uri[]> LookupAsync(ClientConfigItem clientInfo)
		{
#if NETCORE
			return Task.FromResult(GetAddressInternal(clientInfo));
#else
			return TaskHelper.FromResult(GetAddressInternal(clientInfo));
#endif
		}

		public void Dispose()
		{

		}
	}
}
