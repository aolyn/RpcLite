using System;
using System.Threading.Tasks;
using RpcLite.Service;

namespace RpcLite.Client
{
	/// <summary>
	/// 
	/// </summary>
	public interface IRpcClientFilter : IRpcFilter
	{
	}

	/// <summary>
	/// 
	/// </summary>
	public interface IServiceProcessFilter : IRpcClientFilter
	{
		/// <summary>
		/// if filter invoke must call next in Invoke 
		/// </summary>
		Task ProcessAsync(ServiceContext context, Func<ServiceContext, Task> next);
	}

	/// <summary>
	/// 
	/// </summary>
	public interface IServiceInvokeFilter : IRpcClientFilter
	{
		/// <summary>
		/// 
		/// </summary>
		/// <param name="context"></param>
		void BeforeInvoke(ServiceContext context);

		/// <summary>
		/// 
		/// </summary>
		/// <param name="context"></param>
		void AfterInvoke(ServiceContext context);
	}

}
