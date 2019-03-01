using System;
using System.Threading.Tasks;
using RpcLite.Service;

namespace RpcLite.Filter
{
	/// <summary>
	/// 
	/// </summary>
	public interface IActionExecutingFilter : IServiceFilter
	{
		/// <summary>
		/// 
		/// </summary>
		/// <param name="context"></param>
		/// <param name="next"></param>
		Task ProcessAsync(ServiceContext context, Func<ServiceContext, Task> next);
	}
}