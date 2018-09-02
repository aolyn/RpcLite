using System;
using System.Linq;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using RpcLite.Config;

namespace RpcLite.Server.Kestrel
{
	public static class WebHostBuilderRpcLiteExtensions
	{
		public static IWebHostBuilder UseRpcLite(this IWebHostBuilder builder, Action<RpcConfigBuilder> configBuilder)
		{
			var rpcConfig = RpcConfigBuilder.BuildConfig(configBuilder);
			return builder.UseRpcLite(rpcConfig);
		}

		public static IWebHostBuilder UseRpcLite(this IWebHostBuilder builder, RpcConfig rpcConfig)
		{
			var startupAssemblyName = builder.GetSetting(WebHostDefaults.ApplicationKey);
			if (!string.IsNullOrEmpty(startupAssemblyName))
			{
				throw new RpcConfigException("Startup must not be set, if you have Startup Type please directly "
					+ "use AspNetCore and config RpcLite in Startup Type");
			}

			startupAssemblyName = typeof(RpcLiteStartup).Assembly.GetName().Name;
			builder.UseSetting(WebHostDefaults.ApplicationKey, startupAssemblyName);
			var startup = new RpcLiteStartup(rpcConfig);
			builder.ConfigureServices(services =>
			{
				if (services.All(it => it.ServiceType != typeof(IStartup)))
				{
					services.AddSingleton(typeof(IStartup), startup);
				}
			});

			return builder;
		}
	}
}
