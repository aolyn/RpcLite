using RpcLite.Service;

namespace RpcLite.Filter
{
	/// <inheritdoc />
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
}