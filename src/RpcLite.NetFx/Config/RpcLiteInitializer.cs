using System;
using RpcLite.Registry;

namespace RpcLite.Config
{
	public class RpcLiteInitializer
	{
		private static readonly Lazy<object> InitializeService = new Lazy<object>(() =>
		{
			if (RpcLiteConfig.Instance?.Services != null)
			{
				foreach (var service in RpcLiteConfig.Instance.Services)
				{
					RegistryManager.Register(service);
				}
			}

			return null;
		});

		public static void Initialize()
		{
			// ReSharper disable once UnusedVariable
			var value = InitializeService.Value;
		}

	}
}
