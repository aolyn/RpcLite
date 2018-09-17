using RpcLite.Config;

namespace RpcLite.Registry.Consul
{
	public class ConsulRegistryFactory : IRegistryFactory
	{
		public IRegistry CreateRegistry(RpcConfig config)
		{
			return new ConsulRegistry(config);
		}

		public void Dispose()
		{
		}
	}
}
