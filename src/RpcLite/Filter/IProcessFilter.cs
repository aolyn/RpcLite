using System;
using System.Threading.Tasks;
using RpcLite.Service;

namespace RpcLite.Filter
{
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
}