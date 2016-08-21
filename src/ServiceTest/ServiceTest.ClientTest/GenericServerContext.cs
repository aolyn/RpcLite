using System.Collections.Generic;
using System.IO;
using RpcLite.Service;

namespace ServiceTest.ClientTest
{
	public class GenericServerContext : IServerContext
	{
		public IDictionary<string, string> RequestHeader;
		public IDictionary<string, string> ResponseHeader;

		public int RequestContentLength { get; set; }

		public string RequestContentType { get; set; }

		public string RequestPath { get; set; }

		public Stream RequestStream { get; set; }

		public string ResponseContentType { get; set; }

		public int ResponseStatusCode { get; set; }

		public Stream ResponseStream { get; set; }

		public string GetRequestHeader(string key)
		{
			return RequestHeader?[key];
		}

		public string GetResponseHeader(string key)
		{
			return ResponseHeader?[key];
		}

		public void SetResponseHeader(string key, string value)
		{
			ResponseHeader[key] = value;
		}
	}
}
