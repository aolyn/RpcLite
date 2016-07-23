using System;

namespace RpcLite.Registry
{

	public interface IRegistryFactory : IDisposable
	{
		IRegistry CreateRegistry(string address);
	}

}
