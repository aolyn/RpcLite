using System;
using RpcLite.AspNetCore.Service;
using RpcLite.Config;

// ReSharper disable once CheckNamespace
namespace Microsoft.AspNetCore.Routing
{
	/// <summary>
	/// 
	/// </summary>
	public static class RpcLiteRouterBuilderExtensions
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

		/// <summary>
		/// 
		/// </summary>
		/// <param name="app"></param>
		/// <param name="builder"></param>
		/// <returns></returns>
		public static IRouteBuilder UseRpcLite(this IRouteBuilder app, Action<RpcConfigBuilder> builder)
		{
			var builderObj = new RpcConfigBuilder();
			builder(builderObj);
			var config = builderObj.Build();

			return UseRpcLite(app, config);
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="routes"></param>
		/// <param name="rpcConfig"></param>
		/// <returns></returns>
		public static IRouteBuilder UseRpcLite(this IRouteBuilder routes, RpcConfig rpcConfig)
		{
			AspNetCoreInitializer.Initialize(routes, rpcConfig);
			return routes;
		}
	}
}