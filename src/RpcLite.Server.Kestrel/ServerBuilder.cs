using System;
using System.IO;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using RpcLite.Config;

namespace RpcLite.Server.Kestrel
{
	public class ServerBuilder
	{
		private RpcConfig _rpcConfig;
		private Action<IServiceCollection> _configServices;
		private Action<IWebHostBuilder> _configWebHost;
		private string[] _urls;

		public ServerBuilder UseConfig(Action<RpcConfigBuilder> configBuilder)
		{
			_rpcConfig = RpcConfigBuilder.BuildConfig(configBuilder);
			return this;
		}

		public ServerBuilder UseConfig(RpcConfig config)
		{
			_rpcConfig = config;
			return this;
		}

		public ServerBuilder UseUrls(params string[] urls)
		{
			_urls = urls;
			return this;
		}

		public ServerBuilder ConfigureWebHost(Action<IWebHostBuilder> config)
		{
			_configWebHost = config;
			return this;
		}

		public ServerBuilder ConfigureServices(Action<IServiceCollection> configServices)
		{
			if (_configServices != null)
				throw new RpcConfigException("ConfigureServices already called");

			_configServices = configServices;
			return this;
		}

		public Server Build()
		{
			var builder = new WebHostBuilder()
				.UseKestrel()
				.UseLibuv()
				.UseContentRoot(Directory.GetCurrentDirectory());

			_configWebHost?.Invoke(builder);

			var startup = new RpcLiteStartup(_rpcConfig, true);
			var startupAssemblyName = startup.GetType().Assembly.GetName().Name;
			builder.UseSetting(WebHostDefaults.ApplicationKey, startupAssemblyName);
			builder.ConfigureServices(services =>
			{
				services.AddSingleton(typeof(IStartup), startup);
			});

			if (_configServices != null)
			{
				builder.ConfigureServices(_configServices);
			}

			if (_urls != null && _urls.Length > 0)
			{
				builder.UseUrls(_urls);
			}

			var webHost = builder
				.Build();

			var host = new Server(webHost);
			return host;
		}
	}
}