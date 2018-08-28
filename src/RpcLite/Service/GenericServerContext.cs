using System;
using System.Collections.Generic;
using System.IO;

namespace RpcLite.Service
{
	/// <summary>
	/// 
	/// </summary>
	public class GenericServerContext : IServerContext
	{
		/// <summary>
		/// 
		/// </summary>
		public IDictionary<string, string> RequestHeader;

		/// <summary>
		/// 
		/// </summary>
		public IDictionary<string, string> ResponseHeader;

		/// <summary>
		/// 
		/// </summary>
		public int RequestContentLength { get; set; }

		/// <summary>
		/// 
		/// </summary>
		public string RequestContentType { get; set; }

		/// <summary>
		/// 
		/// </summary>
		public string RequestPath { get; set; }

		/// <summary>
		/// 
		/// </summary>
		public Stream RequestStream { get; set; }

		/// <summary>
		/// 
		/// </summary>
		public string ResponseContentType { get; set; }

		/// <summary>
		/// 
		/// </summary>
		public int ResponseStatusCode { get; set; }

		/// <summary>
		/// 
		/// </summary>
		public Stream ResponseStream { get; set; }

#if NETCORE
		/// <summary>
		/// 
		/// </summary>
		public IServiceProvider RequestServices { get; set; }
#endif

		/// <summary>
		/// 
		/// </summary>
		/// <param name="key"></param>
		/// <returns></returns>
		public string GetRequestHeader(string key)
		{
			return RequestHeader?[key];
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="key"></param>
		/// <returns></returns>
		public string GetResponseHeader(string key)
		{
			return ResponseHeader?[key];
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="key"></param>
		/// <param name="value"></param>
		public void SetResponseHeader(string key, string value)
		{
			ResponseHeader[key] = value;
		}
	}
}
