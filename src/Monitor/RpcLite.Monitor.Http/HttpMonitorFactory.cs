using RpcLite.Config;

namespace RpcLite.Monitor.Http
{
	public class HttpMonitorFactory : IMonitorFactory
	{
		public IMonitor CreateMonitor(RpcConfig config)
		{
			return new HttpMonitor(config);
		}
	}

}
