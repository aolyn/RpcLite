using System.Net.Http;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;
using RpcLite;
using RpcLite.Client;
using ServiceTest.Contract;
using Xunit;

namespace ServiceTest.ClientTest
{
	public class CertificateValidationTest
	{
		[Fact]
		public void GetServerAddress()
		{
			var appHost = new AppHostBuilder()
				.UseChannelProvider<HttpChannelProvider>()
				.Build();
			var serviceAddress = "http://localhost:5000/api/service/";
			serviceAddress = "https://www.baidu.com/";
			var gatewayClient = appHost.ClientFactory.GetInstance<IProductService>(serviceAddress);

			var serverInfo = gatewayClient.GetAll();
		}

		public class HttpChannelProvider : DefaultChannelProvider
		{
			public override IChannel GetChannel(string address)
			{
				return new HttpClientChannel(address, CertificateValidate);
			}

			private bool CertificateValidate(HttpRequestMessage httpRequest, X509Certificate2 certificate2,
				X509Chain chain, SslPolicyErrors sslPolicyErrors)
			{
				return true;
			}
		}
	}
}
