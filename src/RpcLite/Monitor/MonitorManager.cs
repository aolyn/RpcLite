using System;
using System.Linq;
using RpcLite.Config;
using RpcLite.Logging;

#if NETCORE
using System.Reflection;
#endif

namespace RpcLite.Monitor
{
	internal class MonitorManager
	{
		public static IMonitor GetMonitor(RpcConfig config)
		{
			var registryItem = config.Monitor;
			if (registryItem == null) return null;

			try
			{
				var type = TypeCreator.GetTypeByIdentifier(registryItem.Type);
				if (type != null)
				{
					var constructors = type
#if NETCORE
						.GetTypeInfo()
						.DeclaredConstructors
						.ToArray();
#else
					.GetConstructors();
#endif
					var constructorWithConfig = constructors
						.Where(it =>
						{
							if (it.IsPrivate)
								return false;

							var paras = it.GetParameters();
							return paras.Length == 1 && paras[0].ParameterType == typeof(RpcConfig);
						})
						.FirstOrDefault();

					if (constructorWithConfig != null)
					{
						var monitor = Activator.CreateInstance(type, config) as IMonitor;
						return monitor;
					}

					var constructorWithoutParamerters = constructors
						.Where(it =>
						{
							if (it.IsPrivate)
								return false;

							var paras = it.GetParameters();
							return paras.Length == 0;
						})
						.FirstOrDefault();

					if (constructorWithoutParamerters != null)
					{
						var monitor = Activator.CreateInstance(type) as IMonitor;
						return monitor;
					}
					return null;
				}
			}
			catch (Exception ex)
			{
				LogHelper.Error("InitializeResolver error", ex);
				throw;
			}

			return null;
		}

	}
}
