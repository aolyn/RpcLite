using RpcLite.Service;

namespace RpcLite.Monitor
{
	public class ConsoleMonitor : IMonitor
	{
		public IMonitorSession GetSession(ServiceContext context)
		{
			return new ConsoleMonitorSession(this);
		}
	}

	public class ConsoleMonitorSession : IMonitorSession
	{
		private IMonitor _monitor;

		public ConsoleMonitorSession(IMonitor monitor)
		{
			_monitor = monitor;
		}

		public void BeginRequest(ServiceContext context)
		{

		}

		public void EndRequest(ServiceContext context)
		{
		}
	}


}
