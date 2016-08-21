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
		private readonly RpcLiteConfig _config;

		public RegistryManager(RpcLiteConfig config)
		{
			_config = config;
			InitializeResolver();
		}

		private void InitializeResolver()
		{
			var registryItem = _config.Registry;
			if (registryItem == null) return;

			try
			{
				var type = TypeCreator.GetTypeFromName(registryItem.TypeName, registryItem.AssemblyName);
				if (type != null)
				{
					_registry = Activator.CreateInstance(type, _config) as IRegistry;
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
		public Uri GetAddress<TContract>()
		{
			if (_registry == null)
				//InitializeResolver();
				return null;

			var type = typeof(TContract);
			var clientConfigItem = _config.Clients
				.FirstOrDefault(it => it.TypeName == type.FullName);

			return _registry?.LookupAsync(clientConfigItem).Result?.FirstOrDefault();
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="serviceInfo"></param>
		public void Register(ServiceConfigItem serviceInfo)
		{
			_registry?.RegisterAsync(serviceInfo).Wait();
		}

	}
}