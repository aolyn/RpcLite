using RpcLite.Service;

namespace RpcLite.Client
{
	/// <summary>
	/// 
	/// </summary>
	public static class ClientFactory
	{
		private static readonly RpcClientFactory Factory = new RpcClientFactory(RpcProcessor.ServiceHost.RegistryManager);

		/// <summary>
		/// 
		/// </summary>
		/// <typeparam name="TContract"></typeparam>
		/// <returns></returns>
		public static TContract GetInstance<TContract>()
			where TContract : class
		{
			return Factory.GetInstance<TContract>();
		}

		/// <summary>
		/// 
		/// </summary>
		/// <typeparam name="TContract"></typeparam>
		/// <returns></returns>
		public static TContract GetInstance<TContract>(string address)
			where TContract : class
		{
			return Factory.GetInstance<TContract>(address);
		}
	}
}
