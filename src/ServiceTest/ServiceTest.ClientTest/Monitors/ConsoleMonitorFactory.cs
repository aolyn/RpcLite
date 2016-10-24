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
		/// <param name="appHost"></param>
		/// <param name="config"></param>
		/// <returns></returns>
		public IMonitor CreateMonitor(AppHost appHost, RpcConfig config)
		{
			return new ConsoleMonitor();
		}
	}
}
