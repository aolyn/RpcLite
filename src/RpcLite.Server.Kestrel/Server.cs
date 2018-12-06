using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;

namespace RpcLite.Server.Kestrel
{
	public class Server
	{
		private readonly IWebHost _webHost;

		public Server(IWebHost webHost)
		{
			_webHost = webHost;
		}

		public void Run()
		{
			_webHost.Run();
		}

		public Task RunAsync()
		{
			return _webHost.RunAsync();
		}

		public void Start()
		{
			_webHost.Start();
		}

		public Task StopAsync()
		{
			return _webHost.StopAsync();
		}
	}
}
