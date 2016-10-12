using RpcLite.Service;

namespace RpcLite.Monitor
{
	/// <summary>
	/// 
	/// </summary>
	public interface IServiceMonitorSession
	{
		/// <summary>
		/// 
		/// </summary>
		/// <param name="context"></param>
		void BeginRequest(ServiceContext context);

		/// <summary>
		/// 
		/// </summary>
		/// <param name="context"></param>
		void EndRequest(ServiceContext context);
	}

}
