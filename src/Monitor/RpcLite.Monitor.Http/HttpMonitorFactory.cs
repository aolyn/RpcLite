using RpcLite.Config;

namespace RpcLite.Monitor.Http
{
	public class HttpMonitorFactory : IMonitorFactory
	{
		public IMonitor CreateMonitor(AppHost appHost, RpcConfig config)
		{
			return new HttpMonitor(appHost, config);
		}
	}

}
