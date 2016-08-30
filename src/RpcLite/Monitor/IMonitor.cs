using RpcLite.Service;

namespace RpcLite.Monitor
{
	/// <summary>
	/// 
	/// </summary>
	public interface IMonitor
	{
		/// <summary>
		/// 
		/// </summary>
		/// <param name="context"></param>
		/// <returns></returns>
		IMonitorSession GetSession(ServiceContext context);
	}
}
