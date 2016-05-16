using System;

namespace RpcLite.Client
{
	/// <summary>
	/// 
	/// </summary>
	public interface IAddressResolver
	{
		/// <summary>
		/// 
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <returns></returns>
		Uri GetAddress<T>() where T : class;

		/// <summary>
		/// 
		/// </summary>
		void OnConfigChanged();
	}
}
