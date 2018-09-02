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
			services.AddRpcLite(_rpcConfig);
			return services.BuildServiceProvider();
		}

		public void Configure(IApplicationBuilder app)
		{
			app.UseRpcLite();

			//Configure(app, app.ApplicationServices.GetService<IHostingEnvironment>(),
			//	app.ApplicationServices.GetService<ILoggerFactory>());
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