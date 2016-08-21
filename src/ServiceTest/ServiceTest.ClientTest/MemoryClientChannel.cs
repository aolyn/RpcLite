using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using RpcLite.Client;
using RpcLite.Net;

namespace ServiceTest.ClientTest
{
	public class MemoryClientChannel : IClientChannel
	{
		public string Address { get; set; }

		public Func<string, Stream, IDictionary<string, string>, Task<ResponseMessage>> ExcuteFunc { get; set; }

		public Task<ResponseMessage> SendAsync(string action, Stream content, IDictionary<string, string> headers)
		{
			return ExcuteFunc(action, content, headers);
		}

	}
}
