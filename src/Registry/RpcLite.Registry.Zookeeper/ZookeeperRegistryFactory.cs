namespace RpcLite.Registry.Zookeeper
{
	public class ZookeeperRegistryFactory : IRegistryFactory
	{
		public IRegistry CreateRegistry(string address)
		{
			return new ZookeeperRegistry(address);
		}

		public void Dispose()
		{
		}
	}
}
