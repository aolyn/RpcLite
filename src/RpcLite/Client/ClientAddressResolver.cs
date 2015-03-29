using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RpcLite.Client
{
	class ClientAddressResolver<T>
	{
		Type type = typeof(T);
		static IAddressResolver Resolver = null;

		static ClientAddressResolver()
		{
			InitializeResolver();
		}

		private static void InitializeResolver()
		{
			var resolverItem = RpcLite.Config.RpcLiteConfigSection.Instance.Resover;
			if (resolverItem != null)
			{
				try
				{
					var type = TypeCreator.GetTypeFromName(resolverItem.TypeName, resolverItem.AssemblyName);
					if (type != null)
					{
						Resolver = Activator.CreateInstance(type) as IAddressResolver;
					}
				}
				catch (Exception ex)
				{
				}
			}
		}

		public static Uri GetAddress()
		{
			if (Resolver == null)
				InitializeResolver();

			return Resolver.GetAddress<T>();
		}
	}

}
