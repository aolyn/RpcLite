using RpcLite.Config;

namespace RpcLite.Registry.Merops
{
	public class MeropsRegistryFactory : IRegistryFactory
	{
		public IRegistry CreateRegistry(AppHost appHost, RpcConfig config)
		{
			return new MeropsRegistry(appHost, config);
		}

		public void Dispose()
		{
		}
	}
}
