namespace RpcLite.Client
{
	/// <summary>
	/// 
	/// </summary>
	public interface IRpcClientFilter : IRpcFilter
	{
	}

	///// <summary>
	///// 
	///// </summary>
	//public interface IClientProcessFilter : IRpcClientFilter
	//{
	//	/// <summary>
	//	/// if filter invoke must call next in Invoke 
	//	/// </summary>
	//	Task ProcessAsync(ServiceContext context, Func<ServiceContext, Task> next);
	//}

	/// <summary>
	/// 
	/// </summary>
	public interface IClientInvokeFilter : IRpcClientFilter
	{
		/// <summary>
		/// 
		/// </summary>
		/// <param name="context"></param>
		void OnInvoking(ClientContext context);

		/// <summary>
		/// 
		/// </summary>
		/// <param name="context"></param>
		void OnInvoked(ClientContext context);
	}

}
