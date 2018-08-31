//using System;
//using RpcLite.AspNetCore.Service;
//using RpcLite.Config;

//// ReSharper disable once CheckNamespace
//namespace Microsoft.AspNetCore.Routing
//{
//	/// <summary>
//	/// 
//	/// </summary>
//	public static class RpcLiteRouterBuilderExtensions
//	{
//		/// <summary>
//		/// 
//		/// </summary>
//		/// <param name="router"></param>
//		/// <param name="builder"></param>
//		/// <returns></returns>
//		public static IRouteBuilder UseRpcLite(this IRouteBuilder router, Action<RpcConfigBuilder> builder)
//		{
//			var config = RpcConfigBuilder.BuildConfig(builder);
//			return UseRpcLite(router, config);
//		}

//		/// <summary>
//		/// 
//		/// </summary>
//		/// <param name="router"></param>
//		/// <returns></returns>
//		public static IRouteBuilder UseRpcLite(this IRouteBuilder router)
//		{
//			AspNetCoreInitializer.Initialize(router);
//			return router;
//		}

//		/// <summary>
//		/// 
//		/// </summary>
//		/// <param name="router"></param>
//		/// <param name="rpcConfig"></param>
//		/// <returns></returns>
//		public static IRouteBuilder UseRpcLite(this IRouteBuilder router, RpcConfig rpcConfig)
//		{
//			AspNetCoreInitializer.Initialize(router, rpcConfig);
//			return router;
//		}
//	}
//}