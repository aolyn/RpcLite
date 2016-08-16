using System;
using System.Threading.Tasks;
using RpcLite.Config;

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
		Task RegisterAsync(ServiceConfigItem serviceInfo);

		/// <summary>
		/// 
		/// </summary>
		/// <param name="clientInfo"></param>
		/// <returns></returns>
		Task<Uri[]> LookupAsync(ClientConfigItem clientInfo);

		/// <summary>
		/// if or not support registry current service to registry server
		/// </summary>
		bool CanRegister { get; }
	}

}
