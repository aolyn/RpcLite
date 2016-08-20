using System;
using System.Configuration;
using RpcLite.Config;
using RpcLite.Registry;

namespace RpcLite.AspNet
{
	/// <summary>
	/// 
	/// </summary>
	public class RpcLiteInitializer
	{
		private static readonly Lazy<object> InitializeService = new Lazy<object>(() =>
		{
			var config = ConfigurationManager.GetSection("RpcLite");
			//RpcLiteConfig.SetInstance()
			if (RpcLiteConfig.Instance?.Services != null)
			{
				foreach (var service in RpcLiteConfig.Instance.Services)
				{
					RegistryManager.Register(service);
				}
			}

			return null;
		});

		/// <summary>
		/// 
		/// </summary>
		public static void Initialize()
		{
			// ReSharper disable once UnusedVariable
			var value = InitializeService.Value;
		}

	}
}
