#if NETCORE
using System;
#endif

using System.IO;

namespace RpcLite.Service
{
	/// <summary>
	/// 
	/// </summary>

	public interface IServerContext
	{
		/// <summary>
		/// 
		/// </summary>
		/// <param name="key"></param>
		/// <returns></returns>
		string GetRequestHeader(string key);

		/// <summary>
		/// 
		/// </summary>
		/// <param name="key"></param>
		/// <param name="value"></param>
		void SetResponseHeader(string key, string value);

		/// <summary>
		/// 
		/// </summary>
		/// <param name="key"></param>
		/// <returns></returns>
		string GetResponseHeader(string key);

		/// <summary>
		/// 
		/// </summary>
		string ResponseContentType { get; set; }

		/// <summary>
		/// 
		/// </summary>
		/// <returns></returns>
		long? ResponseContentLength { get; set; }

		/// <summary>
		/// 
		/// </summary>
		/// <returns></returns>
		string RequestContentType { get; }

		/// <summary>
		/// 
		/// </summary>
		/// <returns></returns>
		Stream RequestStream { get; }

		/// <summary>
		/// 
		/// </summary>
		/// <returns></returns>
		Stream ResponseStream { get; }

		/// <summary>
		/// 
		/// </summary>
		string RequestPath { get; }

		/// <summary>
		/// 
		/// </summary>
		int RequestContentLength { get; }

		/// <summary>
		/// 
		/// </summary>
		int ResponseStatusCode { get; set; }

#if NETCORE
		/// <summary>
		/// services container for this request
		/// </summary>
		IServiceProvider RequestServices { get; set; }
#endif
	}
}
