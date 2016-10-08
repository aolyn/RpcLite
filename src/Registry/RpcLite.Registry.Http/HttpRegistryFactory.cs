using RpcLite.Config;

namespace RpcLite.Registry.Http
{
	public class HttpRegistryFactory : IRegistryFactory
	{
		public IRegistry CreateRegistry(AppHost appHost, RpcConfig config)
		{
			return new HttpRegistry(appHost, config);
		}

		public void Dispose()
		{
		}
	}
}
