#if NETCORE

using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;

namespace RpcLite.Service
{
	// You may need to install the Microsoft.AspNet.Http.Abstractions package into your project
	public class RpcLiteMiddleware
	{
		private readonly RequestDelegate _next;

		public RpcLiteMiddleware(RequestDelegate next)
		{
			_next = next;
		}

		public async Task Invoke(HttpContext httpContext)
		{
			if (await RpcProcessor.ProcessAsync(new AspNetCoreServerContext(httpContext))) return;

			await _next(httpContext);
		}
	}

	// Extension method used to add the middleware to the HTTP request pipeline.
	public static class RpcLiteMiddlewareExtensions
	{
		public static IApplicationBuilder UseRpcLiteMiddleware(this IApplicationBuilder builder)
		{
			return builder.UseMiddleware<RpcLiteMiddleware>();
		}
	}

}

#endif