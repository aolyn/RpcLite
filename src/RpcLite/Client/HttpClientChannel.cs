using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;
using RpcLite.Net;
using RpcLite.Service;

namespace RpcLite.Client
{
	/// <summary>
	/// 
	/// </summary>
	public class HttpClientChannel : IChannel
	{
		private readonly HttpClient _httpClient;

		/// <summary>
		/// 
		/// </summary>
		public string Address { get; set; }

		/// <summary>
		/// 
		/// </summary>
		/// <param name="address"></param>
		public HttpClientChannel(string address) : this(null, address)
		{
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="httpClient"></param>
		/// <param name="address"></param>
		public HttpClientChannel(HttpClient httpClient, string address)
		{
			_httpClient = httpClient ?? new HttpClient();
			Address = address;
		}

#if NETCORE
		/// <summary>
		/// 
		/// </summary>
		/// <param name="address"></param>
		/// <param name="certValidate"></param>
		public HttpClientChannel(string address,
			Func<HttpRequestMessage, X509Certificate2, X509Chain, SslPolicyErrors, bool> certValidate)
		{
			if (certValidate == null) throw new ArgumentNullException(nameof(certValidate));

			var handler = new HttpClientHandler
			{
				ClientCertificateOptions = ClientCertificateOption.Manual,
				ServerCertificateCustomValidationCallback = certValidate
			};
			_httpClient = new HttpClient(handler);
			Address = address;
		}
#endif

		/// <inheritdoc />
		/// <summary>
		/// </summary>
		/// <param name="action"></param>
		/// <param name="content"></param>
		/// <param name="headers"></param>
		/// <returns></returns>
		public Task<IResponseMessage> SendAsync(string action, Stream content, IDictionary<string, string> headers)
		{
			if (string.IsNullOrWhiteSpace(Address))
				throw new ServiceException("service address not provided");

			var url = Address + action;
			return WebRequestHelper.PostAsync(_httpClient, url, content, headers);
		}

	}
}
