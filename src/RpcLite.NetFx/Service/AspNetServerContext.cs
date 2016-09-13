#if !NETCORE

using System.IO;
using System.Web;

namespace RpcLite.Service
{
	/// <summary>
	/// 
	/// </summary>
	public class AspNetServerContext : IServerContext
	{
		private readonly HttpContext _httpContext;

		/// <summary>
		/// 
		/// </summary>
		/// <param name="httpContext"></param>
		public AspNetServerContext(HttpContext httpContext)
		{
			_httpContext = httpContext;
		}

		private string _requestPath;

		/// <summary>
		/// 
		/// </summary>
		public string RequestPath
		{
			get
			{
				if (_requestPath == null)
				{
					_requestPath = _httpContext.Request.QueryString.Count == 0
						? _httpContext.Request.Path
						: _httpContext.Request.Path + "?" + _httpContext.Request.QueryString;

					if (_httpContext.Request.ApplicationPath?.Length > 1)
						_requestPath = _requestPath.Substring(_httpContext.Request.ApplicationPath.Length);
				}
				return _requestPath;
			}
		}

		/// <summary>
		/// 
		/// </summary>
		public int RequestContentLength => _httpContext.Request.ContentLength;

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
		public Stream RequestStream => _httpContext.Request.InputStream;

		/// <summary>
		/// 
		/// </summary>
		public Stream ResponseStream => _httpContext.Response.OutputStream;

		/// <summary>
		/// 
		/// </summary>
		public int ResponseStatusCode
		{
			get { return _httpContext.Response.StatusCode; }
			set { _httpContext.Response.StatusCode = value; }
		}

	}
}

#endif