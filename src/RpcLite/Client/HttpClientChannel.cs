using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using RpcLite.Net;

namespace RpcLite.Client
{
	public class HttpClientChannel : IClientChannel
	{
		public string Address { get; set; }

		public HttpClientChannel(string address)
		{
			Address = address;
		}

		public Task<ResponseMessage> SendAsync(string action, Stream content, IDictionary<string, string> headers)
		{
			var url = Address + action;
			return WebRequestHelper.PostAsync(url, content, headers);
		}
	}
}
