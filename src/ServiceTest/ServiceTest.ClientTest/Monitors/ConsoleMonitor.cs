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
		public IServiceMonitorSession GetServiceSession(ServiceContext context)
		{
			return new ConsoleMonitorSession();
		}

		/// <summary>
		/// 
		/// </summary>
		/// <returns></returns>
		public IClientMonitorSession GetClientSession()
		{
			return null;
		}

		/// <summary>
		/// 
		/// </summary>
		public void Dispose()
		{

		}
	}

	/// <summary>
	/// 
	/// </summary>
	internal class ConsoleMonitorSession : IServiceMonitorSession
	{
		//private IMonitor _monitor;

		///// <summary>
		///// 
		///// </summary>
		///// <param name="monitor"></param>
		//public ConsoleMonitorSession(IMonitor monitor)
		//{
		//	_monitor = monitor;
		//}

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
