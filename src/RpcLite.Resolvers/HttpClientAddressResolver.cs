using System;
using System.Linq;
using System.Reflection;
using RpcLite.Client;
using RpcLite.Config;
using RpcLite.Logging;
using ServiceRegistry.Contract;

namespace RpcLite.Resolvers
{
	public class HttpClientAddressResolver : IAddressResolver
	{
		private static readonly object InitializeLocker = new object();
		private static QuickReadConcurrentDictionary<Type, string> _defaultBaseUrlDictionary = new QuickReadConcurrentDictionary<Type, string>();

		static HttpClientAddressResolver()
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
			var tempDic = new QuickReadConcurrentDictionary<Type, string>();
			foreach (var item in RpcLiteConfig.Instance.Clients)
			{
				Assembly assembly;

				//find ClientType Assembly
				if (!string.IsNullOrWhiteSpace(item.AssemblyName))
				{
#if NETCORE
					assembly = Assembly.Load(new AssemblyName(item.AssemblyName));
#else
					var asms = AppDomain.CurrentDomain.GetAssemblies();
					assembly = asms.FirstOrDefault(it => it.FullName.StartsWith(item.AssemblyName + ",", StringComparison.OrdinalIgnoreCase))
						?? Assembly.Load(item.AssemblyName);
#endif
				}
				else
				{
					assembly = Assembly.GetEntryAssembly();
				}

				var typeInfo = assembly.GetType(item.TypeName);

				if (!string.IsNullOrWhiteSpace(item.Path))
					tempDic.Add(typeInfo, item.Path);
			}

			_defaultBaseUrlDictionary = tempDic;
		}

		public Uri GetAddress<TClient>() where TClient : class
		{
			return GetAddressInternal<TClient>();
		}

		public Uri GetAddress(Type clientType)
		{
			return GetAddressInternal(clientType);
		}

		private static Lazy<RpcClientBase<IRegistryService>> _registryClient =
			new Lazy<RpcClientBase<IRegistryService>>(() =>
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


		private static Uri GetAddressInternal<T>() where T : class
		{
			var type = typeof(T);
			return GetAddressInternal(type);
		}

		private static Uri GetAddressInternal(Type type)
		{
			// ReSharper disable once InconsistentlySynchronizedField
			var url = _defaultBaseUrlDictionary.GetOrAdd(type, () =>
			{
				var clientConfigItem = RpcLiteConfig.Instance.Clients
					.FirstOrDefault(it => it.TypeName == type.FullName);

				if (clientConfigItem == null) return null;
				if (_registryClient.Value == null) return null;

				var request = new GetServiceAddressRequest
				{
					ServiceName = clientConfigItem.Name,
					Namespace = clientConfigItem.Namespace,
					Environment = RpcLiteConfig.Instance.ClientEnvironment,
				};
				var response = _registryClient.Value.Client.GetServiceAddress(request);
				var uri = string.IsNullOrWhiteSpace(response?.Address)
					? null
					: response.Address;

				return uri;
			});

			return string.IsNullOrEmpty(url)
				? null
				: new Uri(url);
		}

		public void OnConfigChanged()
		{
			InitilizeBaseUrlsSafe();
		}

	}
}
