using System;
using System.Net;

namespace RpcLite.Net
{
	internal class WebProxy : IWebProxy
	{
		private readonly Uri _uri;

		public WebProxy(string uri)
		{
			_uri = new Uri(uri);
		}

		public ICredentials Credentials { get; set; }

		public Uri GetProxy(Uri destination)
		{
			return _uri;
		}

		public bool IsBypassed(Uri host)
		{
			return false;
		}

	}
}