using System;
using System.Threading.Tasks;

namespace RpcLite.Registry
{
	public interface IRegistry : IDisposable
	{
		/// <summary>
		/// 
		/// </summary>
		/// <param name="appId"></param>
		/// <param name="address"></param>
		/// <returns></returns>
		Task RegisterAsync(string appId, string[] address);

		Task<string[]> LookupAsync(string appId);

		/// <summary>
		/// if or not support registry current service to registry server
		/// </summary>
		bool CanRegister { get; }
	}

}
