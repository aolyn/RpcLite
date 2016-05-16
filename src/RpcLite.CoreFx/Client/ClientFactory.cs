namespace RpcLite.Client
{
	/// <summary>
	/// 
	/// </summary>
	public static class ClientFactory
	{
		/// <summary>
		/// 
		/// </summary>
		/// <typeparam name="TContract"></typeparam>
		/// <returns></returns>
		public static RpcClientBase<TContract> GetInstance<TContract>()
			where TContract : class
		{
			return RpcClientBase<TContract>.GetInstance();
		}

		/// <summary>
		/// 
		/// </summary>
		/// <typeparam name="TContract"></typeparam>
		/// <returns></returns>
		public static RpcClientBase<TContract> GetInstance<TContract>(string url)
			where TContract : class
		{
			return RpcClientBase<TContract>.GetInstance(url);
		}

	}
}
