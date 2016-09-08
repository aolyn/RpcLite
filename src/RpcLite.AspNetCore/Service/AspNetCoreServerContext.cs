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

		/// <summary>
		/// 
		/// </summary>
		/// <param name="httpContext"></param>
		public AspNetCoreServerContext(HttpContext httpContext)
		{
			_httpContext = httpContext;
		}

		/// <summary>
		/// 
		/// </summary>
		public string RequestPath => _httpContext.Request.QueryString.HasValue
			? _httpContext.Request.Path + _httpContext.Request.QueryString.Value
			: _httpContext.Request.Path.Value;

		/// <summary>
		/// 
		/// </summary>
		public int RequestContentLength => (int)(_httpContext.Request.ContentLength ?? 0);

		/// <summary>
		/// 
		/// </summary>
		public string ResponseContentType
		{
			get { return _httpContext.Response.ContentType; }
			set { _httpContext.Response.ContentType = value; }
		}

		/// <summary>
		/// 
		/// </summary>
		public string RequestContentType => _httpContext.Request.ContentType;

		/// <summary>
		/// 
		/// </summary>
		public Stream RequestStream => _httpContext.Request.Body;

		/// <summary>
		/// 
		/// </summary>
		public Stream ResponseStream => _httpContext.Response.Body;

		/// <summary>
		/// 
		/// </summary>
		public int ResponseStatusCode
		{
			get { return _httpContext.Response.StatusCode; }
			set { _httpContext.Response.StatusCode = value; }
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="key"></param>
		/// <returns></returns>
		public string GetRequestHeader(string key)
		{
			return _httpContext.Request.Headers[key];
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="key"></param>
		/// <param name="value"></param>
		public void SetResponseHeader(string key, string value)
		{
			_httpContext.Response.Headers[key] = value;
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="key"></param>
		/// <returns></returns>
		public string GetResponseHeader(string key)
		{
			return _httpContext.Response.Headers[key];
		}

	}
}

#endif