using System;
using System.Threading.Tasks;

namespace RpcLite.Registry
{
	/// <summary>
	/// 
	/// </summary>
	public interface IRegistry : IDisposable
	{
		/// <summary>
		/// 
		/// </summary>
		/// <param name="serviceInfo"></param>
		/// <returns></returns>
		Task RegisterAsync(ServiceInfo serviceInfo);

		/// <summary>
		/// search first config item by TContract and then lookup
		/// </summary>
		/// <returns></returns>
		Task<ServiceInfo[]> LookupAsync<TContract>();

		/// <summary>
		/// search config item by name and then lookup
		/// </summary>
		/// <returns></returns>
		Task<ServiceInfo[]> LookupAsync(string name);

		/// <summary>
		/// lookup by name and group
		/// </summary>
		/// <returns></returns>
		Task<ServiceInfo[]> LookupAsync(string name, string group);

		/// <summary>
		/// if or not support registry current service to registry server
		/// </summary>
		bool CanRegister { get; }
	}

}
