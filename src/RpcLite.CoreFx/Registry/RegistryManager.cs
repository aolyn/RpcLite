using System;
using System.Linq;
using RpcLite.Config;
using RpcLite.Logging;

namespace RpcLite.Registry
{
	/// <summary>
	/// 
	/// </summary>
	public class RegistryManager
	{
		// ReSharper disable once StaticMemberInGenericType
		private static IRegistry _registry;

		static RegistryManager()
		{
			InitializeResolver();
		}

		private static void InitializeResolver()
		{
			var registryItem = RpcLiteConfig.Instance.Registry;
			if (registryItem == null) return;

			try
			{
				var type = TypeCreator.GetTypeFromName(registryItem.TypeName, registryItem.AssemblyName);
				if (type != null)
				{
					_registry = Activator.CreateInstance(type) as IRegistry;
				}
			}
			catch (Exception ex)
			{
				LogHelper.Error("InitializeResolver error", ex);
				throw;
			}
		}

		/// <summary>
		/// 
		/// </summary>
		/// <typeparam name="TContract"></typeparam>
		/// <returns></returns>
		public static Uri GetAddress<TContract>()
		{
			if (_registry == null)
				//InitializeResolver();
				return null;

			var type = typeof(TContract);
			var clientConfigItem = RpcLiteConfig.Instance.Clients
				.FirstOrDefault(it => it.TypeName == type.FullName);

			return _registry?.LookupAsync(clientConfigItem).Result?.FirstOrDefault();
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="serviceInfo"></param>
		public static void Register(ServiceConfigItem serviceInfo)
		{
			_registry?.RegisterAsync(serviceInfo).Wait();
		}

	}
}