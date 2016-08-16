#if !NETCORE

using System.IO;
using System.Web;

namespace RpcLite.Service
{
	public class AspNetServerContext : IServerContext
	{
		private readonly HttpContext _httpContext;

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

		public AspNetServerContext(HttpContext httpContext)
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
			return _httpContext.Request.InputStream;
		}

		public Stream GetResponseStream()
		{
			return _httpContext.Response.OutputStream;
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