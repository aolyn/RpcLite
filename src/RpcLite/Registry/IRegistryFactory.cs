using System;
using RpcLite.Config;

namespace RpcLite.Registry
{

	/// <summary>
	/// 
	/// </summary>
	public interface IRegistryFactory : IDisposable
	{
		/// <summary>
		/// 
		/// </summary>
		/// <param name="appHost"></param>
		/// <param name="config"></param>
		/// <returns></returns>
		IRegistry CreateRegistry(AppHost appHost, RpcConfig config);
	}

}
