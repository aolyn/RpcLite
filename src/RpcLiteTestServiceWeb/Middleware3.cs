using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;

namespace RpcLiteTestServiceWeb
{
	// You may need to install the Microsoft.AspNetCore.Http.Abstractions package into your project
	public class Middleware3
	{
		private readonly RequestDelegate _next;

		public Middleware3(RequestDelegate next)
		{
			_next = next;
		}

		public Task Invoke(HttpContext httpContext)
		{
			using (var writer = new StreamWriter(httpContext.Response.Body))
			{
				writer.WriteLine("hello");
			}

			return _next(httpContext);
		}
	}

	// Extension method used to add the middleware to the HTTP request pipeline.
	public static class Middleware3Extensions
	{
		public static IApplicationBuilder UseMiddleware3(this IApplicationBuilder builder)
		{
			return builder.UseMiddleware<Middleware3>();
		}
	}
}
