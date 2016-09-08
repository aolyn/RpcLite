using RpcLite.Service;

namespace RpcLite.Client
{
	/// <summary>
	/// 
	/// </summary>
	public static class ClientFactory
	{
		//private static readonly RpcClientFactory Factory = new RpcClientFactory(RpcProcessor.ServiceHost.RegistryManager);

		/// <summary>
		/// 
		/// </summary>
		/// <typeparam name="TContract"></typeparam>
		/// <returns></returns>
		public static TContract GetInstance<TContract>()
			where TContract : class
		{
			return AppHost.ClientFactory.GetInstance<TContract>();
		}

		/// <summary>
		/// 
		/// </summary>
		/// <typeparam name="TContract"></typeparam>
		/// <returns></returns>
		public static TContract GetInstance<TContract>(string address)
			where TContract : class
		{
			return AppHost.ClientFactory.GetInstance<TContract>(address);
		}

		private static AppHost AppHost
		{
			get
			{
				if (RpcManager.AppHost == null)
					throw new NotInitializedException("RpcLite not initialized");

				return RpcManager.AppHost;
			}
		}

	}
}
