using System;
using System.Net;
using System.Reflection;
using System.Threading.Tasks;
using Microsoft.AspNet.Builder;
using Microsoft.AspNet.Http;
using RpcLite.Logging;

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
			var request = httpContext.Request;
			var response = httpContext.Response;

			if (request.Path != "/")
			{
				var serviceContext = new ServiceContext
				{
					Request = new ServiceRequest
					{
						RequestStream = request.Body,
						Path = request.Path,
						ContentType = request.ContentType,
						ContentLength = (int)(request.ContentLength ?? 0),
					},
					Response = new ServiceResponse
					{
						ResponseStream = response.Body,
					},
					//ExtraData = extraData,
					ExecutingContext = httpContext,
				};

				try
				{
					var result = RpcProcessor.ProcessAsync(serviceContext);
					await result;
					EndProcessRequestInternal(serviceContext);
					return;
				}
				catch (Exception ex)
				{
					LogHelper.Error("process request error in RpcLiteMiddleware", ex);
				}
			}

			await _next(httpContext);
		}

		private void EndProcessRequestInternal(ServiceContext context)
		{
			//LogHelper.Debug("RpcAsyncHandler.EndProcessRequest start RpcService.EndProcessRequest(result);");

			//var context = result.AsyncState as ServiceContext;
			if (context == null)
			{
				//LogHelper.Error("ServiceContext is null", null);
				return;
			}

			var httpContext = (HttpContext)context.ExecutingContext;
			httpContext.Response.ContentType = context.Response.ContentType;
			if (context.Exception != null)
			{
				httpContext.Response.Headers["RpcLite-ExceptionType"] = context.Exception.GetType().FullName;
				httpContext.Response.Headers["RpcLite-ExceptionAssembly"] = context.Exception.GetType().GetTypeInfo().Assembly.FullName;
				httpContext.Response.Headers["RpcLite-StatusCode"] = ((int)HttpStatusCode.InternalServerError).ToString();
				httpContext.Response.StatusCode = (int)HttpStatusCode.InternalServerError;

				if (context.Formatter == null)
					throw context.Exception;

				context.Formatter.Serialize(context.Response.ResponseStream, context.Exception);
			}
			else
			{
				httpContext.Response.Headers["RpcLite-StatusCode"] = ((int)HttpStatusCode.OK).ToString();

				if (context.Result != null && context.Action.HasReturnValue)
					context.Formatter.Serialize(context.Response.ResponseStream, context.Result);
			}

			//LogHelper.Debug("RpcAsyncHandler.EndProcessRequest end RpcService.EndProcessRequest(result);");
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

