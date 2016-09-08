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
		/// <param name="config"></param>
		/// <returns></returns>
		IMonitor CreateMonitor(RpcConfig config);
	}
}
