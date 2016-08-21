#if NETCORE

using System.IO;
using Microsoft.AspNetCore.Http;

namespace RpcLite.Service
{
	/// <summary>
	/// 
	/// </summary>
	public class AspNetCoreServerContext : IServerContext
	{
		private readonly HttpContext _httpContext;

		public AspNetCoreServerContext(HttpContext httpContext)
		{
			_httpContext = httpContext;
		}

		public string RequestPath => _httpContext.Request.QueryString.HasValue
			? _httpContext.Request.Path + _httpContext.Request.QueryString.Value
			: _httpContext.Request.Path.Value;

		public int RequestContentLength => (int)(_httpContext.Request.ContentLength ?? 0);

		public string ResponseContentType
		{
			get { return _httpContext.Response.ContentType; }
			set { _httpContext.Response.ContentType = value; }
		}

		public string RequestContentType => _httpContext.Request.ContentType;

		public Stream RequestStream => _httpContext.Request.Body;

		public Stream ResponseStream => _httpContext.Response.Body;

		public int ResponseStatusCode
		{
			get { return _httpContext.Response.StatusCode; }
			set { _httpContext.Response.StatusCode = value; }
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

	}
}

#endif