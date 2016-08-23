#if NETCORE

using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;

namespace RpcLite.Service
{
	// You may need to install the Microsoft.AspNet.Http.Abstractions package into your project
	/// <summary>
	/// 
	/// </summary>
	public class RpcLiteMiddleware
	{
		private readonly RequestDelegate _next;

		/// <summary>
		/// 
		/// </summary>
		/// <param name="next"></param>
		public RpcLiteMiddleware(RequestDelegate next)
		{
			_next = next;
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="httpContext"></param>
		/// <returns></returns>
		public async Task Invoke(HttpContext httpContext)
		{
			if (await RpcManager.ProcessAsync(new AspNetCoreServerContext(httpContext))) return;

			await _next(httpContext);
		}
	}

	// Extension method used to add the middleware to the HTTP request pipeline.
	/// <summary>
	/// 
	/// </summary>
	public static class RpcLiteMiddlewareExtensions
	{
		/// <summary>
		/// 
		/// </summary>
		/// <param name="builder"></param>
		/// <returns></returns>
		public static IApplicationBuilder UseRpcLiteMiddleware(this IApplicationBuilder builder)
		{
			return builder.UseMiddleware<RpcLiteMiddleware>();
		}
	}

}

#endif