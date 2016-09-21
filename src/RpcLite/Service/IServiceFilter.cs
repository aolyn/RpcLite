using System;
using System.Threading.Tasks;

namespace RpcLite.Service
{
	/// <summary>
	/// 
	/// </summary>
	public interface IServiceFilter : IRpcFilter
	{
	}

	/// <summary>
	/// 
	/// </summary>
	public interface IProcessFilter : IServiceFilter
	{
		/// <summary>
		/// if filter invoke must call next in Invoke 
		/// </summary>
		Task ProcessAsync(ServiceContext context, Func<ServiceContext, Task> next);
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

	/// <summary>
	/// 
	/// </summary>
	public interface IServiceInvokeFilter : IServiceFilter
	{
		/// <summary>
		/// 
		/// </summary>
		/// <param name="context"></param>
		void OnInvoking(ServiceContext context);

		/// <summary>
		/// 
		/// </summary>
		/// <param name="context"></param>
		void OnInvoked(ServiceContext context);
	}

	/// <summary>
	/// 
	/// </summary>
	public interface IActionExecteFilter : IServiceFilter
	{
		/// <summary>
		/// 
		/// </summary>
		/// <param name="context"></param>
		void OnExecuting(ServiceContext context);

		/// <summary>
		/// 
		/// </summary>
		/// <param name="context"></param>
		void OnExecuted(ServiceContext context);
	}

}
