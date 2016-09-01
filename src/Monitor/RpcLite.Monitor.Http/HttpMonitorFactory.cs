using RpcLite.Config;

namespace RpcLite.Monitor.Http
{
	public class HttpMonitorFactory : IMonitorFactory
	{
		public IMonitor CreateMonitor(RpcLiteConfig config)
		{
			return new HttpMonitor(config);
		}
	}

}
