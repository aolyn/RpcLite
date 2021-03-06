﻿using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
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
	public static class RpcLiteIApplicationBuilderExtensions
	{
		/// <summary>
		/// default initialize from rpclite.config.json
		/// </summary>
		/// <param name="app"></param>
		/// <returns></returns>
		public static IApplicationBuilder UseRpcLite(this IApplicationBuilder app)
		{
			var config = (RpcConfig)app.ApplicationServices.GetService(typeof(RpcConfig));
			return app.UseRpcLite(config);
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="app"></param>
		/// <param name="builder"></param>
		/// <returns></returns>
		public static IApplicationBuilder UseRpcLite(this IApplicationBuilder app, Action<RpcConfigBuilder> builder)
		{
			var config = RpcConfigBuilder.BuildConfig(builder);
			return app.UseRpcLite(config);
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="app"></param>
		/// <param name="rpcConfig"></param>
		/// <returns></returns>
		public static IApplicationBuilder UseRpcLite(this IApplicationBuilder app, RpcConfig rpcConfig)
		{
			Initialize(app, rpcConfig);
			return app;
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

			var routerBuilder = new RouteBuilder(app);
			MapService(rpcConfig.Service.Services, routerBuilder, appHost);
			MapServiceInfoHandler(rpcConfig, routerBuilder);
			var routes = routerBuilder.Build();
			app.UseRouter(routes);
		}

		private static void MapServiceInfoHandler(RpcConfig rpcConfig, IRouteBuilder routerBuilder)
		{
			if (rpcConfig?.Service?.Services == null)
				return;

			routerBuilder.MapRoute("rpcliteinfo", async context =>
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
			});
		}

		private static void MapService(List<ServiceConfigItem> services, IRouteBuilder routers, AppHost appHost)
		{
			const string nullOperation = "_DefaultNullOperationName";
			foreach (var service in services)
			{
				routers.MapRoute(service.Path + $"{{RpcOperationName={nullOperation}}}/{{*RpcPathInfo}}", context =>
				{
					context.Response.Headers[HeaderNames.Connection] = "Keep-Alive";

					//var content = System.Text.Encoding.UTF8.GetBytes("mock3 - 1 from RpcLite test run!");
					//context.Response.ContentLength = content.Length;
					//return context.Response.Body.WriteAsync(content, 0, content.Length);

					var operationName = (string)context.GetRouteValue("RpcOperationName");
					if (operationName == nullOperation) operationName = null;

					var pathInfo = (string)context.GetRouteValue("RpcPathInfo");
					var serverContext = new AspNetCoreServerContext(context)
					{
						RequestPathInfo = new RequestPathInfo
						{
							Service = service.Name,
							Action = operationName,
							Query = context.Request.QueryString.ToString(),
							PathInfo = pathInfo,
						}
					};

					return appHost.ProcessAsync(serverContext);
				});
			}
		}
	}
}