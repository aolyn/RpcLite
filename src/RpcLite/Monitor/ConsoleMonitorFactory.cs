using RpcLite.Config;

namespace RpcLite.Monitor
{
	/// <summary>
	/// 
	/// </summary>
	public class ConsoleMonitorFactory : IMonitorFactory
	{
		/// <summary>
		/// 
		/// </summary>
		/// <param name="config"></param>
		/// <returns></returns>
		public IMonitor CreateMonitor(RpcConfig config)
		{
			return new ConsoleMonitor();
		}
	}
}
