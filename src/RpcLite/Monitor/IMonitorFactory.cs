using RpcLite.Config;

namespace RpcLite.Monitor
{
	/// <summary>
	/// 
	/// </summary>
	public interface IMonitorFactory
	{
		/// <summary>
		/// 
		/// </summary>
		/// <param name="appHost"></param>
		/// <param name="config"></param>
		/// <returns></returns>
		IMonitor CreateMonitor(AppHost appHost, RpcConfig config);
	}
}
