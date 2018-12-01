using System;
using System.Threading;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using RpcLite.Config;

namespace RpcLite.Server.Kestrel
{
	internal class RpcLiteStartup : IStartup
	{
		private static int _startupIndex;
		private readonly RpcConfig _rpcConfig;
		private readonly bool _selfHost;

		public RpcLiteStartup(RpcConfig configServices) : this(configServices, false)
		{
		}

		public RpcLiteStartup(RpcConfig configServices, bool selfHost)
		{
			Interlocked.Increment(ref _startupIndex);
			_rpcConfig = configServices;
			_selfHost = selfHost;
		}

		public IServiceProvider ConfigureServices(IServiceCollection services)
		{
			if (!_selfHost)
				services.AddRouting();
			if (_rpcConfig != null)
				services.AddRpcLite(_rpcConfig);
			else
				services.AddRpcLite();
			return services.BuildServiceProvider();
		}

		public void Configure(IApplicationBuilder app)
		{
			if (_selfHost)
			{
#pragma warning disable 618
				PathMatchRpcLiteIApplicationBuilder.ConfigSelfHost(app);
#pragma warning restore 618
			}
			else
			{
				app.UseRpcLite();
			}

			var appHost = app.ApplicationServices.GetService<AppHost>();
			if (appHost != null)
			{
				var appLifeTime = app.ApplicationServices.GetService<IApplicationLifetime>();
				appLifeTime.ApplicationStopped.Register(() => { appHost.Stop(); });
			}
		}
	}
}