#if !NETCORE

using System.IO;
using System.Web;
using RpcLite.Service;

namespace RpcLite.AspNet.Service
{
	/// <summary>
	/// 
	/// </summary>
	public class AspNetServerContext : IServerContext
	{
		private readonly HttpContext _httpContext;
		private const string HeadPrefix = "RpcLite-";

		/// <inheritdoc />
		public AspNetServerContext(HttpContext httpContext)
		{
			_httpContext = httpContext;
		}

		private string _requestPath;

		/// <inheritdoc />
		public string RequestPath
		{
			get
			{
				if (_requestPath != null) return _requestPath;

				var path = _httpContext.Request.Path.StartsWith("/")
					? _httpContext.Request.Path.Substring(1)
					: _httpContext.Request.Path;

				_requestPath = _httpContext.Request.QueryString.Count == 0
					? path
					: path + "?" + _httpContext.Request.QueryString;

				if (_httpContext.Request.ApplicationPath?.Length > 1)
					_requestPath = _requestPath.Substring(_httpContext.Request.ApplicationPath.Length);

				return _requestPath;
			}
		}

		/// <inheritdoc />
		public int RequestContentLength => _httpContext.Request.ContentLength;

		/// <inheritdoc />
		public string GetRequestHeader(string key)
		{
			return _httpContext.Request.Headers[HeadPrefix + key];
		}

		/// <inheritdoc />
		public void SetResponseHeader(string key, string value)
		{
			_httpContext.Response.Headers[HeadPrefix + key] = value;
		}

		/// <inheritdoc />
		public string GetResponseHeader(string key)
		{
			return _httpContext.Response.Headers[HeadPrefix + key];
		}

		/// <inheritdoc />
		public string ResponseContentType
		{
			get => _httpContext.Response.ContentType;
			set => _httpContext.Response.ContentType = value;
		}

		/// <inheritdoc />
		public long? ResponseContentLength
		{
			get
			{
				var header = _httpContext.Response.Headers["ContentLength"];
				return header != null && long.TryParse(header, out var len)
					? len
					: -1L;
			}
			set => _httpContext.Response.Headers["ContentLength"] = value.ToString();
		}

		/// <inheritdoc />
		public string RequestContentType => _httpContext.Request.ContentType;

		/// <inheritdoc />
		public Stream RequestStream => _httpContext.Request.InputStream;

		/// <inheritdoc />
		public Stream ResponseStream => _httpContext.Response.OutputStream;

		/// <inheritdoc />
		public int ResponseStatusCode
		{
			get => _httpContext.Response.StatusCode;
			set => _httpContext.Response.StatusCode = value;
		}

	}
}

#endif