using System;
using System.IO;
using Microsoft.AspNetCore.Hosting;
using RpcLite.Config;

namespace RpcLite.Server.Kestrel
{
	public class HostBuilder
	{
		private Action<RpcConfigBuilder> _configBuilder;
		private string[] _urls;

		public HostBuilder UseConfig(Action<RpcConfigBuilder> configBuilder)
		{
			_configBuilder = configBuilder;
			return this;
		}

		public HostBuilder UseUrls(params string[] urls)
		{
			_urls = urls;
			return this;
		}

		public Host Build()
		{
			var startupType = StartupBuilder.Create(_configBuilder);

			var builder = new WebHostBuilder()
				.UseKestrel()
				.UseContentRoot(Directory.GetCurrentDirectory())
				.UseKestrel()
				.UseStartup(startupType);

			if (_urls != null && _urls.Length > 0)
			{
				builder.UseUrls(_urls);
			}

			var webHost = builder
				.Build();

			var host = new Host(webHost);
			return host;
		}
	}
}
