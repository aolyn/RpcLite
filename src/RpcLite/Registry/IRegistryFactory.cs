using System;

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
		/// <param name="address"></param>
		/// <returns></returns>
		IRegistry CreateRegistry(string address);
	}

}
