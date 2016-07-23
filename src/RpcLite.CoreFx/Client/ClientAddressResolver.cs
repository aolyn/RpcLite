using System;
using RpcLite.Config;
using RpcLite.Logging;

namespace RpcLite.Client
{
	internal class ClientAddressResolver<T> where T : class
	{
		// ReSharper disable once StaticMemberInGenericType
		private static IAddressResolver _resolver;

		static ClientAddressResolver()
		{
			InitializeResolver();
		}

		private static void InitializeResolver()
		{
			var resolverItem = RpcLiteConfig.Instance.Resover;
			if (resolverItem == null) return;

			try
			{
				var type = TypeCreator.GetTypeFromName(resolverItem.TypeName, resolverItem.AssemblyName);
				if (type != null)
				{
					_resolver = Activator.CreateInstance(type) as IAddressResolver;
				}
			}
			catch (Exception ex)
			{
				LogHelper.Error("InitializeResolver error", ex);
				throw;
			}
		}

		public static Uri GetAddress()
		{
			if (_resolver == null)
				InitializeResolver();

			return _resolver?.GetAddress<T>();
		}
	}

}
