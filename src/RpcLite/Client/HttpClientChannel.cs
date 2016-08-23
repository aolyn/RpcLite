using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using RpcLite.Net;

namespace RpcLite.Client
{
	/// <summary>
	/// 
	/// </summary>
	public class HttpClientChannel : IClientChannel
	{
		/// <summary>
		/// 
		/// </summary>
		public string Address { get; set; }

		/// <summary>
		/// 
		/// </summary>
		/// <param name="address"></param>
		public HttpClientChannel(string address)
		{
			Address = address;
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="action"></param>
		/// <param name="content"></param>
		/// <param name="headers"></param>
		/// <returns></returns>
		public Task<ResponseMessage> SendAsync(string action, Stream content, IDictionary<string, string> headers)
		{
			var url = Address + action;
			return WebRequestHelper.PostAsync(url, content, headers);
		}
	}
}
