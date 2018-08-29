using System;
using RpcLite.AspNetCore.Service;
using RpcLite.Config;

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
			var builderObj = new RpcConfigBuilder();
			builder(builderObj);
			var config = builderObj.Build();

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
			AspNetCoreInitializer.Initialize(app, rpcConfig);
			return app;
		}

	}
}