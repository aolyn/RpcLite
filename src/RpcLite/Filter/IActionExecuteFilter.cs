using RpcLite.Service;

namespace RpcLite.Filter
{
	/// <summary>
	/// 
	/// </summary>
	public interface IActionExecuteFilter : IServiceFilter
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