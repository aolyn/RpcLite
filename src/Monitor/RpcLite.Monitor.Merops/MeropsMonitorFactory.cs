using RpcLite.Config;

namespace RpcLite.Monitor.Merops
{
	public class MeropsMonitorFactory : IMonitorFactory
	{
		public IMonitor CreateMonitor(AppHost appHost, RpcConfig config)
		{
			return new MeropsMonitor(appHost, config);
		}
	}

}
