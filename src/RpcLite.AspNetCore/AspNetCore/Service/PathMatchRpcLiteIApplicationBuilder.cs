using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.Net.Http.Headers;
using RpcLite;
using RpcLite.AspNetCore.Service;
using RpcLite.Config;
using RpcLite.Service;

// ReSharper disable once CheckNamespace
namespace Microsoft.AspNetCore.Builder
{
	/// <summary>
	/// 
	/// </summary>
	[Obsolete("SelfHost use only")]
	public class PathMatchRpcLiteIApplicationBuilder
	{
		/// <summary>
		/// default initialize from rpclite.config.json
		/// </summary>
		/// <param name="app"></param>
		/// <returns></returns>
		public static void ConfigSelfHost(IApplicationBuilder app)
		{
			var config = (RpcConfig)app.ApplicationServices.GetService(typeof(RpcConfig));
			Initialize(app, config);
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="app"></param>
		/// <param name="rpcConfig"></param>
		private static void Initialize(IApplicationBuilder app, RpcConfig rpcConfig)
		{
			var appHost = (AppHost)app?.ApplicationServices.GetService(typeof(AppHost));
			if (appHost == null) throw new ServiceException("AddRpcLite not called in Startup.ConfigureServices");

			MapService(app, rpcConfig, appHost);
		}

		private static async Task RpcLiteInfoHandler(RpcConfig rpcConfig, HttpContext context)
		{
			await context.Response.WriteAsync(@"<style>
a:visited{
	color: blue;
}
td, th{
	text-align: left;
}
</style>

<p><b>RpcLite Services</b></p>
<table>
	<tr><th>Service Name</th><th>Path</th></tr>
");
			foreach (var item in rpcConfig.Service.Services)
			{
				await context.Response.WriteAsync($"\r\n<tr><td>{item.Name}</td>"
					+ $"<td><a href=\"{item.Path}\">{item.Path}</a></td></tr>");
			}

			await context.Response.WriteAsync("</table>");
		}

		private static void MapService(IApplicationBuilder app, RpcConfig config, AppHost appHost)
		{
			var rpcliteInfoPath = new PathString("/rpcliteinfo");
			var mappings = config.Service.Services
				.Select(it =>
				{
					var path = it.Path.TrimEnd('/');
					path = it.Path.StartsWith("/") ? path : "/" + path;

					return new
					{
						Path = path,
						Service = it,
					};
				})
				.OrderByDescending(it => it.Path)
				.ThenByDescending(it => it.Path.Length)
				.ToArray();

			app.Run(async context =>
			{
				context.Response.Headers[HeaderNames.Connection] = "Keep-Alive";

				if (context.Request.Path.StartsWithSegments(rpcliteInfoPath))
				{
					await RpcLiteInfoHandler(config, context);
					return;
				}

				var serverContext = new AspNetCoreServerContext(context);
				var serviceInfo = mappings.FirstOrDefault(it =>
					(context.Request.Path.Value.Length > it.Path.Length
						&& context.Request.Path.Value[it.Path.Length] == '/'
						|| context.Request.Path.Value.Length == it.Path.Length)
					&& context.Request.Path.StartsWithSegments(it.Path));
				if (serviceInfo != null)
				{
					var action = context.Request.Path.Value.Length > serviceInfo.Path.Length
						? context.Request.Path.Value.Substring(serviceInfo.Path.Length + 1)
						: null;
					serverContext.RequestPathInfo = new RequestPathInfo
					{
						Service = serviceInfo.Service.Name,
						Action = action,
						Query = context.Request.QueryString.Value
					};
				}

				var isServiceRequest = await appHost.ProcessAsync(serverContext);

				if (!isServiceRequest)
				{
					await context.Response.WriteAsync("RpcLite is running");
				}
			});
		}
	}
}