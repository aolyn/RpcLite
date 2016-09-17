using System;
using RpcLite.AspNetCore;
using RpcLite.Config;

// ReSharper disable once CheckNamespace
namespace Microsoft.AspNetCore.Routing
{
	/// <summary>
	/// 
	/// </summary>
	public static class AspNetCoreRpcLiteInitializerExtenstions
	{
		/// <summary>
		/// 
		/// </summary>
		/// <param name="routes"></param>
		/// <returns></returns>
		public static IRouteBuilder UseRpcLite(this IRouteBuilder routes)
		{
			RpcInitializer.Initialize(routes);
			return routes;
		}
	}
}

namespace Microsoft.AspNetCore.Builder
{
	/// <summary>
	/// 
	/// </summary>
	public static class AspNetCoreRpcLiteIApplicationBuilderExtenstions
	{
		/// <summary>
		/// 
		/// </summary>
		/// <param name="app"></param>
		/// <returns></returns>
		public static IApplicationBuilder UseRpcLite(this IApplicationBuilder app)
		{
			RpcInitializer.Initialize(app);
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

			RpcInitializer.Initialize(app, config);
			return app;
		}

	}
}
