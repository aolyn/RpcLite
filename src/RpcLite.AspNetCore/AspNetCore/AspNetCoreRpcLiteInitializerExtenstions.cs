using RpcLite.AspNetCore;

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
			RpcLiteInitializer.Initialize(routes);
			return routes;
		}
	}
}
