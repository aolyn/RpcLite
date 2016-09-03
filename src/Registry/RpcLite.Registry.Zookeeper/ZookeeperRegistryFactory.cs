using RpcLite.Config;

namespace RpcLite.Registry.Zookeeper
{
	public class ZookeeperRegistryFactory : IRegistryFactory
	{
		public IRegistry CreateRegistry(RpcConfig config)
		{
			return new ZookeeperRegistry(config);
		}

		public void Dispose()
		{
		}
	}
}
