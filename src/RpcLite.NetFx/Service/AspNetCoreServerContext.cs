#if NETCORE

using System.IO;
using Microsoft.AspNetCore.Http;

namespace RpcLite.Service
{
	public class AspNetCoreServerContext : IServerContext
	{
		private readonly HttpContext _httpContext;

		public string RequestPath => _httpContext.Request.QueryString.HasValue
			? _httpContext.Request.Path + _httpContext.Request.QueryString.Value
			: _httpContext.Request.Path.Value;

		public int RequestContentLength => (int)(_httpContext.Request.ContentLength ?? 0);

		public AspNetCoreServerContext(HttpContext httpContext)
		{
			_httpContext = httpContext;
		}

		public string GetRequestHeader(string key)
		{
			return _httpContext.Request.Headers[key];
		}

		public void SetResponseHeader(string key, string value)
		{
			_httpContext.Response.Headers[key] = value;
		}

		public string GetResponseHeader(string key)
		{
			return _httpContext.Response.Headers[key];
		}

		public void SetResponseContentType(string type)
		{
			_httpContext.Response.ContentType = type;
		}

		public string GetResponseContentType(string type)
		{
			return _httpContext.Response.ContentType;
		}

		public string GetRequestContentType(string type)
		{
			return _httpContext.Request.ContentType;
		}

		public Stream GetRequestStream()
		{
			return _httpContext.Request.Body;
		}

		public Stream GetResponseStream()
		{
			return _httpContext.Response.Body;
		}

		public string GetResponseContentType()
		{
			return _httpContext.Response.ContentType;
		}

		public string GetRequestContentType()
		{
			return _httpContext.Request.ContentType;
		}

		public void SetResponseStatusCode(int statusCode)
		{
			_httpContext.Response.StatusCode = statusCode;
		}
	}
}

#endif