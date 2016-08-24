using RpcLite.Service;

namespace RpcLite.Monitor
{
	public interface IMonitorSession
	{
		void BeginRequest(ServiceContext context);
		void EndRequest(ServiceContext context);
	}

}
