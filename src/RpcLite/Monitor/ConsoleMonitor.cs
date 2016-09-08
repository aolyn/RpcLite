using RpcLite.Service;

namespace RpcLite.Monitor
{
	/// <summary>
	/// 
	/// </summary>
	public class ConsoleMonitor : IMonitor
	{
		/// <summary>
		/// 
		/// </summary>
		/// <param name="context"></param>
		/// <returns></returns>
		public IMonitorSession GetSession(ServiceContext context)
		{
			return new ConsoleMonitorSession(this);
		}
	}

	/// <summary>
	/// 
	/// </summary>
	public class ConsoleMonitorSession : IMonitorSession
	{
		private IMonitor _monitor;

		/// <summary>
		/// 
		/// </summary>
		/// <param name="monitor"></param>
		public ConsoleMonitorSession(IMonitor monitor)
		{
			_monitor = monitor;
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="context"></param>
		public void BeginRequest(ServiceContext context)
		{

		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="context"></param>
		public void EndRequest(ServiceContext context)
		{
		}
	}

}
