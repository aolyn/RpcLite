using System;
using RpcLite.Service;

namespace RpcLite.Monitor
{
	/// <summary>
	/// 
	/// </summary>
	public interface IMonitor : IDisposable
	{
		/// <summary>
		/// 
		/// </summary>
		/// <param name="context"></param>
		/// <returns></returns>
		IServiceMonitorSession GetServiceSession(ServiceContext context);

		/// <summary>
		/// returns null if not support client monitor
		/// </summary>
		/// <returns></returns>
		IClientMonitorSession GetClientSession();
	}
}
