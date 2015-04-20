using System;
using System.Linq;
using System.Reflection;
using RpcLite.Client;
using RpcLite.Config;
using ServiceRegistery.Contract;

namespace RpcLite.Resolvers
{
	public class HttpClientAddressResolver : IAddressResolver
	{
		private static readonly object initializeLocker = new object();
		private static QuickReadConcurrentDictionary<Type, string> _defaultBaseUrlDictionary = new QuickReadConcurrentDictionary<Type, string>();

		static HttpClientAddressResolver()
		{
			InitilizeBaseUrlsSafe();
		}

		private static void InitilizeBaseUrlsSafe()
		{
			lock (initializeLocker)
			{
				InitilizeBaseUrls();
			}
		}

		private static void InitilizeBaseUrls()
		{
			var tempDic = new QuickReadConcurrentDictionary<Type, string>();
			foreach (var item in RpcLiteConfigSection.Instance.Clients)
			{
				Assembly assembly;

				if (!string.IsNullOrWhiteSpace(item.AssemblyName))
				{
					var asms = AppDomain.CurrentDomain.GetAssemblies();
					assembly = asms.FirstOrDefault(it => it.FullName.StartsWith(item.AssemblyName + ",", StringComparison.OrdinalIgnoreCase))
						?? Assembly.Load(item.AssemblyName);
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
			//defaultBaseUrlDictionary.Clear();
			//foreach (var item in tempDic)
			//{
			//	defaultBaseUrlDictionary.Add(item.Key, item.Value);
			//}
			//tempDic.Clear();
		}

		public Uri GetAddress<T>() where T : class
		{
			return GetAddressInternal<T>();
		}

		private static Uri GetAddressInternal<T>() where T : class
		{
			// ReSharper disable once InconsistentlySynchronizedField
			var url = _defaultBaseUrlDictionary.GetOrAdd(typeof(T), () =>
			{
				var clientConfigItem = RpcLiteConfigSection.Instance.Clients
					.FirstOrDefault(it => it.TypeName == typeof(T).FullName);

				if (clientConfigItem != null)
				{
					var registryClientConfigItem = RpcLiteConfigSection.Instance.Clients
						.FirstOrDefault(it => it.TypeName == typeof(IRegistryService).FullName);

					if (registryClientConfigItem != null && !string.IsNullOrWhiteSpace(registryClientConfigItem.Path))
					{
						var client = RpcClientBase<IRegistryService>.GetInstance(registryClientConfigItem.Path);
						var request = new GetServiceAddressRequest
						{
							ServiceName = clientConfigItem.Name,
							Namespace = clientConfigItem.Namespace,
							Environment = RpcLiteConfigSection.Instance.ClientEnvironment,
						};
						var response = client.Client.GetServiceAddress(request);
						var uri = response == null || string.IsNullOrWhiteSpace(response.Address)
							? null
							: response.Address;

						return uri;
					}
				}

				return null;
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
