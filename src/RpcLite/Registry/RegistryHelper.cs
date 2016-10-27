using System;
using System.Collections.Generic;
using RpcLite.Config;
using RpcLite.Logging;

namespace RpcLite.Registry
{
	/// <summary>
	/// 
	/// </summary>
	internal static class RegistryHelper
	{
		internal static IRegistry GetRegistry(AppHost appHost, RpcConfig config)
		{
			var registryItem = config.Registry;
			if (registryItem == null)
				return new DefaultRegistry(config);

			try
			{
				var type = ReflectHelper.GetTypeByIdentifier(registryItem.Type);
				if (type == null)
					throw new ConfigException($"can't get type '{registryItem.Type}' from registry config");

				var factory = Activator.CreateInstance(type) as IRegistryFactory;
				if (factory == null)
				{
					throw new ConfigException(@"registry type not implements IRegistryFactory");
				}
				var registry = factory.CreateRegistry(appHost, config);
				return registry;
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
		/// <typeparam name="TDictionary"></typeparam>
		/// <param name="config"></param>
		/// <returns></returns>
		public static TDictionary GetAddresses<TDictionary>(RpcConfig config)
			where TDictionary : IDictionary<ServiceIdentifier, ServiceInfo[]>, new()
		{
			if (config.Client?.Clients == null)
				return new TDictionary();

			var tempDic = new TDictionary();
			foreach (var item in config.Client?.Clients)
			{
				if (!string.IsNullOrWhiteSpace(item.Address))
					tempDic.Add(new ServiceIdentifier(item.Name, item.Group), new[]
					{
						new ServiceInfo
						{
							Name = item.Name,
							Group = item.Group,
							Address = item.Address
						}
					});
			}

			return tempDic;
		}

	}
}