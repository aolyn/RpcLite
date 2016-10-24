using System;
using RpcLite.AspNetCore;
using RpcLite.Config;

// ReSharper disable once CheckNamespace
namespace Microsoft.AspNetCore.Routing
{
	/// <summary>
	/// 
	/// </summary>
	public static class RpcLiteRouterBuilderExtenstions
	{
		/// <summary>
		/// 
		/// </summary>
		/// <param name="routes"></param>
		/// <returns></returns>
		public static IRouteBuilder UseRpcLite(this IRouteBuilder routes)
		{
			AspNetCoreInitializer.Initialize(routes);
			return routes;
		}
	}
}

namespace Microsoft.AspNetCore.Builder
{
	/// <summary>
	/// 
	/// </summary>
	public static class RpcLiteIApplicationBuilderExtenstions
	{
		/// <summary>
		/// default initialize from rpclite.config.json
		/// </summary>
		/// <param name="app"></param>
		/// <returns></returns>
		public static IApplicationBuilder UseRpcLite(this IApplicationBuilder app)
		{
			AspNetCoreInitializer.Initialize(app);
			return app;
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="app"></param>
		/// <param name="builder"></param>
		/// <returns></returns>
		public static IApplicationBuilder UseRpcLite(this IApplicationBuilder app, Action<RpcConfigBuilder> builder)
		{
			var builderObj = new RpcConfigBuilder();
			builder(builderObj);
			var config = builderObj.Build();

			AspNetCoreInitializer.Initialize(app, config);
			return app;
		}

	}
}
