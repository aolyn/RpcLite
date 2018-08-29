namespace RpcLite.Filter
{
	/// <summary>
	/// 
	/// </summary>
	public interface IServiceFilter : IRpcFilter
	{
	}

	///// <summary>
	///// 
	///// </summary>
	//public interface IActionFilter : IServiceFilter
	//{
	//	/// <summary>
	//	/// if filter invoke must call next in Invoke 
	//	/// </summary>
	//	Task ProcessAsync(ServiceContext context, Func<ServiceContext, Task> next);
	//}
}
