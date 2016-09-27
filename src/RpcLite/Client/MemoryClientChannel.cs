using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Threading.Tasks;
using RpcLite.Service;

namespace RpcLite.Client
{
	/// <summary>
	/// 
	/// </summary>
	public class MemoryClientChannel : IClientChannel
	{
		private readonly AppHost _appHost;

		/// <summary>
		/// 
		/// </summary>
		public string Address { get; set; }

		/// <summary>
		/// 
		/// </summary>
		public Func<string, Stream, IDictionary<string, string>, Task<ResponseMessage>> SendAsyncFunc { get; set; }

		/// <summary>
		/// 
		/// </summary>
		/// <param name="appHost"></param>
		public MemoryClientChannel(AppHost appHost)
		{
			_appHost = appHost;
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="action"></param>
		/// <param name="content"></param>
		/// <param name="headers"></param>
		/// <returns></returns>
		public virtual Task<ResponseMessage> SendAsync(string action, Stream content, IDictionary<string, string> headers)
		{
			if (SendAsyncFunc != null)
				return SendAsyncFunc(action, content, headers);

			var outputStream = new MemoryStream();
			var responseHeader = new Dictionary<string, string>();

			var request = new GenericServerContext
			{
				RequestContentLength = (int)content.Length,
				RequestContentType = headers["Content-Type"],
				RequestPath = Address + action,
				RequestStream = content,
				RequestHeader = headers,

				ResponseStream = outputStream,
				ResponseHeader = responseHeader,
			};

			return _appHost.ProcessAsync(request).ContinueWith(tsk =>
			{
				var processed = tsk.Result;
				if (!processed)
					return null;

				string statusCode;
				var isSuccess = responseHeader.TryGetValue(HeaderName.StatusCode, out statusCode)
					&& statusCode == RpcStatusCode.Ok;

				outputStream.Position = 0;
				var response = new ResponseMessage(null)
				{
					Headers = responseHeader,
					IsSuccess = isSuccess,
					Result = outputStream,
				};

				return response;
			});

		}

	}
}
