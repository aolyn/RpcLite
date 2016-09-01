using RpcLite.Config;

namespace RpcLite.Registry.Http
{
	public class HttpRegistryFactory : IRegistryFactory
	{
		public void Dispose()
		{
		}

		public IRegistry CreateRegistry(RpcLiteConfig config)
		{
			return new HttpRegistry(config);
		}
	}
}
