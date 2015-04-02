using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using RpcLite.Client;
using RpcLite.Config;
using ServiceRegistery.Contract;

namespace RpcLite.Resolvers
{
	public class HttpClientAddressResolver : IAddressResolver
	{
		private static bool _isInitializing;
		private static readonly Dictionary<Type, string> defaultBaseUrlDictionary = new Dictionary<Type, string>();

		static HttpClientAddressResolver()
		{
			InitilizeBaseUrlsSafe();
		}

		private static readonly object initializeLocker = new object();
		private static void InitilizeBaseUrlsSafe()
		{
			lock (initializeLocker)
			{
				_isInitializing = true;
				InitilizeBaseUrls();
				_isInitializing = false;
			}
		}

		private static void InitilizeBaseUrls()
		{
			var tempDic = new Dictionary<Type, string>();
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

			defaultBaseUrlDictionary.Clear();
			foreach (var item in tempDic)
			{
				defaultBaseUrlDictionary.Add(item.Key, item.Value);
			}
			tempDic.Clear();
		}

		public Uri GetAddress<T>() where T : class
		{
			return GetAddressInternal<T>();
		}


		//TODO: thread safe
		private static Uri GetAddressInternal<T>() where T : class
		{
			//RpcLiteConfigSection.Instance.Clients[0].TypeName 

			string baseUrl;
			if (defaultBaseUrlDictionary.TryGetValue(typeof(T), out baseUrl))
			{
				return new Uri(baseUrl);
			}

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
					return string.IsNullOrWhiteSpace(response.Address)
						? null
						: new Uri(response.Address);
				}
			}

			return null;
		}

		public void OnConfigChanged()
		{
			InitilizeBaseUrlsSafe();
		}
	}
}
