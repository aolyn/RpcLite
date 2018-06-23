using System;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using RpcLite.Config;

namespace RpcLite.Server.Kestrel
{
	internal class RpcLiteStartup
	{
		public static Action<RpcConfigBuilder> ConfigBuilder { get; set; }

		public void ConfigureServices(IServiceCollection services)
		{
			services.AddRouting();
		}

		public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
		{
			app.UseRpcLite(ConfigBuilder);

			app.Run(async context =>
			{
				await context.Response.WriteAsync("RpcLite Server is running");
			});
		}
	}
}