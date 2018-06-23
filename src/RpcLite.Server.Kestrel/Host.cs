using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;

namespace RpcLite.Server.Kestrel
{
	public class Host
	{
		private readonly IWebHost _webHost;

		public Host(IWebHost webHost)
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
	}
}
