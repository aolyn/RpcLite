using RpcLite.Config;

namespace RpcLite.Registry.Merops
{
	public class MeropsRegistryFactory : IRegistryFactory
	{
		public IRegistry CreateRegistry(RpcConfig config)
		{
			return new MeropsRegistry(config);
		}

		public void Dispose()
		{
		}
	}
}
