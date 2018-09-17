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

		public RpcLiteStartup(RpcConfig configServices)
		{
			Interlocked.Increment(ref _startupIndex);
			_rpcConfig = configServices;
		}

		public IServiceProvider ConfigureServices(IServiceCollection services)
		{
			services.AddRouting();
			if (_rpcConfig != null)
				services.AddRpcLite(_rpcConfig);
			else
				services.AddRpcLite();
			return services.BuildServiceProvider();
		}

		public void Configure(IApplicationBuilder app)
		{
			app.UseRpcLite();

			var appHost = app.ApplicationServices.GetService<AppHost>();
			if (appHost != null)
			{
				var appLifeTime = app.ApplicationServices.GetService<IApplicationLifetime>();
				appLifeTime.ApplicationStopped.Register(() => { appHost.Stop(); });
			}
		}

		//private void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
		//{
		//	app.UseRpcLite();

		//	app.Run(async context =>
		//	{
		//		await context.Response.WriteAsync("RpcLite Server is running");
		//	});
		//}
	}
}