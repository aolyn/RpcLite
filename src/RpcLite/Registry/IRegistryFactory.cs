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
		/// <param name="config"></param>
		/// <returns></returns>
		IRegistry CreateRegistry(RpcConfig config);
	}

}
