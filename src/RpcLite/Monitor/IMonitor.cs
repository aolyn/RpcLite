using RpcLite.Service;

namespace RpcLite.Monitor
{
	/// <summary>
	/// 
	/// </summary>
	public interface IMonitor
	{
		IMonitorSession GetSession(ServiceContext context);
	}
}
