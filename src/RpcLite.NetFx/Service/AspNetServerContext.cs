#if !NETCORE

using System.IO;
using System.Web;

namespace RpcLite.Service
{
	public class AspNetServerContext : IServerContext
	{
		private readonly HttpContext _httpContext;

		public AspNetServerContext(HttpContext httpContext)
		{
			_httpContext = httpContext;
		}

		private string _requestPath;
		public string RequestPath
		{
			get
			{
				if (_requestPath == null)
				{
					_requestPath = _httpContext.Request.QueryString.Count == 0
						? _httpContext.Request.Path
						: _httpContext.Request.Path + "?" + _httpContext.Request.QueryString;
				}
				return _requestPath;
			}
		}

		public int RequestContentLength => _httpContext.Request.ContentLength;

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

		public string ResponseContentType
		{
			get { return _httpContext.Response.ContentType; }
			set { _httpContext.Response.ContentType = value; }
		}

		public string RequestContentType => _httpContext.Request.ContentType;

		public Stream RequestStream => _httpContext.Request.InputStream;

		public Stream ResponseStream => _httpContext.Response.OutputStream;

		public int ResponseStatusCode
		{
			get { return _httpContext.Response.StatusCode; }
			set { _httpContext.Response.StatusCode = value; }
		}

	}
}

#endif