using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using RpcLite.Client;
using RpcLite.Config;

namespace RpcLite.Resolvers
{
	public class HttpClientAddressResolver : IAddressResolver
	{
		private static bool isInitializing = false;
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
				isInitializing = true;
				InitilizeBaseUrls();
				isInitializing = false;
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

				tempDic.Add(typeInfo, item.Path);
			}

			defaultBaseUrlDictionary.Clear();
			foreach (var item in tempDic)
			{
				defaultBaseUrlDictionary.Add(item.Key, item.Value);
			}
			tempDic.Clear();
		}

		public Uri GetAddress<T>()
		{
			return GetAddressInternal<T>();
		}

		private static Uri GetAddressInternal<T>()
		{
			string baseUrl;
			return defaultBaseUrlDictionary.TryGetValue(typeof(T), out baseUrl)
				? new Uri(baseUrl)
				: null;
		}

		public void OnConfigChanged()
		{
			InitilizeBaseUrlsSafe();
		}
	}
}
