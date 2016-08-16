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
		public static TContract GetInstance<TContract>()
			where TContract : class
		{
			return RpcClientBase<TContract>.GetInstance() as TContract;
		}

		/// <summary>
		/// 
		/// </summary>
		/// <typeparam name="TContract"></typeparam>
		/// <returns></returns>
		public static TContract GetInstance<TContract>(string url)
			where TContract : class
		{
			return RpcClientBase<TContract>.GetInstance(url) as TContract;
		}

	}
}
