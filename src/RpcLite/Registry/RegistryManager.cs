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
		private readonly RpcConfig _config;

		/// <summary>
		/// 
		/// </summary>
		/// <param name="config"></param>
		public RegistryManager(RpcConfig config)
		{
			_config = config;
			InitializeRegistry();
		}

		private void InitializeRegistry()
		{
			var registryItem = _config.Registry;
			if (registryItem == null) return;

			try
			{
				var type = TypeCreator.GetTypeByIdentifier(registryItem.Type);
				if (type != null)
				{
					var factory = Activator.CreateInstance(type) as IRegistryFactory;
					if (factory == null)
					{
						throw new ConfigException(@"registry type not implements IRegistryFactory");
					}
					_registry = factory.CreateRegistry(_config);
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
		public string GetAddress<TContract>()
		{
			if (_registry == null)
				//InitializeResolver();
				return null;

			var type = typeof(TContract);
			var clientConfigItem = _config.Client.Clients
				.FirstOrDefault(it => it.TypeName == type.FullName);

			return clientConfigItem == null
				? null
				: _registry?.LookupAsync(clientConfigItem).Result?.FirstOrDefault();
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="serviceInfo"></param>
		public void Register(ServiceConfigItem serviceInfo)
		{
			if (_registry?.CanRegister ?? false)
				_registry?.RegisterAsync(serviceInfo).Wait();
		}

	}
}