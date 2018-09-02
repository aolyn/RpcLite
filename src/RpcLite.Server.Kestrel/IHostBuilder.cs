using System;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using RpcLite.Config;

namespace RpcLite.Server.Kestrel
{
	public interface IHostBuilder
	{
		HostBuilder UseConfig(Action<RpcConfigBuilder> configBuilder);

		HostBuilder UseConfig(RpcConfig config);

		HostBuilder UseUrls(params string[] urls);

		HostBuilder ConfigureServices(Action<IServiceCollection> configServices);

		HostBuilder UseWebHostBuilder(IWebHostBuilder webHostBuilder);

		Host Build();
	}
}