using System;
using System.IO;
using System.Linq;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using RpcLite.Config;

namespace RpcLite.Server.Kestrel
{
	public class HostBuilder : IHostBuilder
	{
		private RpcConfig _rpcConfig;
		private Action<IServiceCollection> _configServices;
		private IWebHostBuilder _webHostBuilder;
		private string[] _urls;

		public HostBuilder UseConfig(Action<RpcConfigBuilder> configBuilder)
		{
			_rpcConfig = RpcConfigBuilder.BuildConfig(configBuilder);
			return this;
		}

		public HostBuilder UseConfig(RpcConfig config)
		{
			_rpcConfig = config;
			return this;
		}

		public HostBuilder UseUrls(params string[] urls)
		{
			_urls = urls;
			return this;
		}

		public HostBuilder ConfigureServices(Action<IServiceCollection> configServices)
		{
			if (_configServices != null)
				throw new RpcConfigException("ConfigureServices already called");

			_configServices = configServices;
			return this;
		}

		public HostBuilder UseWebHostBuilder(IWebHostBuilder webHostBuilder)
		{
			_webHostBuilder = webHostBuilder;
			return this;
		}

		public Host Build()
		{
			var builder = _webHostBuilder
				?? new WebHostBuilder()
					.UseKestrel()
					.UseContentRoot(Directory.GetCurrentDirectory());

			var startupAssemblyName = builder.GetSetting(WebHostDefaults.ApplicationKey);
			if (!string.IsNullOrEmpty(startupAssemblyName))
			{
				throw new RpcConfigException("Startup must not be set, if you have Startup Type please directly "
					+ "use AspNetCore and config in Startup Type");
			}

			startupAssemblyName = typeof(RpcLiteStartup).Assembly.GetName().Name;
			builder.UseSetting(WebHostDefaults.ApplicationKey, startupAssemblyName);
			var startup = new RpcLiteStartup(_rpcConfig);
			builder.ConfigureServices(services =>
			{
				if (services.All(it => it.ServiceType != typeof(IStartup)))
				{
					services.AddSingleton(typeof(IStartup), startup);
				}
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

			var host = new Host(webHost);
			return host;
		}
	}
}