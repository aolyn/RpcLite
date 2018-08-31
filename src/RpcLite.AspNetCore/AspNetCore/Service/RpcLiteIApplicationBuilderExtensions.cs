using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Routing;
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
			if (appHost == null) throw new ServiceException("AddRpcLite not called in ConfigureService");

			var routersBuilder = new RouteBuilder(app);
			MapService(rpcConfig.Service.Services, routersBuilder, appHost);
			var routes = routersBuilder.Build();
			app.UseRouter(routes);
		}

		private static void MapService(List<ServiceConfigItem> serviceServices, IRouteBuilder routers, AppHost appHost)
		{
			foreach (var path in serviceServices)
			{
				routers.MapRoute(path.Path + "{*RpcLiteServicePath}",
					context => appHost.ProcessAsync(new AspNetCoreServerContext(context)));
			}
		}

	}
}