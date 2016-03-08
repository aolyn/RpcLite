using System.Net;
using System.Text;
using Newtonsoft.Json;

namespace WebApiClient
{
	public class WebApiClient
	{
		public string BaseUrl;
		public string RequestUrl;
		public string ContentType;

		public WebApiClient(string baseUrl, string requestUrl, string contentType)
		{
			BaseUrl = baseUrl;
			RequestUrl = requestUrl;
			ContentType = contentType;
		}

		public R Get<R>()
		{
			var url = BaseUrl + RequestUrl;
			return Get<R>(url);
		}

		private R Get<R>(string url)
		{
			var client = WebRequest.Create(url) as HttpWebRequest;
			client.Accept = ContentType;
			client.Method = "GET";
			var response = client.GetResponse();
			var stm = response.GetResponseStream();
			byte[] buf = new byte[response.ContentLength];
			var rl = stm.Read(buf, 0, buf.Length);
			var sjson = Encoding.UTF8.GetString(buf);
			var robj = JsonConvert.DeserializeObject<R>(sjson);
			return robj;
		}

		public R Get<P, R>(P arg)
		{
			return Get<R>(BaseUrl + RequestUrl + "/" + arg.ToString());
		}

		public R Post<P, R>(P arg)
		{
			var url = BaseUrl + RequestUrl;
			var method = "POST";
			var contentType = ContentType;

			return Invoke<P, R>(arg, url, method, contentType);
		}

		public void Post<P>(P arg)
		{
			var url = BaseUrl + RequestUrl;
			var method = "POST";
			var contentType = ContentType;

			Invoke<P, object>(arg, url, method, contentType, false);
		}

		public R Put<P, R>(P arg)
		{
			var url = BaseUrl + RequestUrl;
			var method = "PUT";
			var contentType = ContentType;

			return Invoke<P, R>(arg, url, method, contentType);
		}

		public void Put<P>(P arg)
		{
			var url = BaseUrl + RequestUrl;
			const string method = "PUT";
			var contentType = ContentType;

			Invoke<P, object>(arg, url, method, contentType, false);
		}

		public R Delete<P, R>(P arg)
		{
			var url = BaseUrl + RequestUrl;
			var method = "DELETE";
			var contentType = ContentType;

			return Invoke<P, R>(arg, url, method, contentType);
		}

		public void Delete<P>(P arg)
		{
			var url = BaseUrl + RequestUrl + "/" + arg.ToString();
			const string method = "DELETE";
			var contentType = ContentType;

			Invoke<P, object>(arg, url, method, contentType, false, false);
		}

		private R Invoke<P, R>(P arg, string url, string method, string contentType)
		{
			return Invoke<P, R>(arg, url, method, contentType, true);
		}

		private R Invoke<P, R>(P arg, string url, string method, string contentType, bool hasReturnValue)
		{
			return Invoke<P, R>(arg, url, method, contentType, true, hasReturnValue);
		}

		private R Invoke<P, R>(P arg, string url, string method, string contentType, bool hasBody, bool hasReturnValue)
		{
			var request = CreateRequest(url, method, contentType);

			if (hasBody)
			{
				var sReqJson = JsonConvert.SerializeObject(arg);
				var sbysbtes = Encoding.UTF8.GetBytes(sReqJson);

				request.ContentLength = sbysbtes.Length;
				var reqStream = request.GetRequestStream();

				reqStream.Write(sbysbtes, 0, sbysbtes.Length);
				reqStream.Flush();
			}
			//reqStream.Close();

			var response = request.GetResponse();
			if (hasReturnValue)
			{
				var stm = response.GetResponseStream();
				byte[] buf = new byte[response.ContentLength];
				var rl = stm.Read(buf, 0, buf.Length);
				var sjson = Encoding.UTF8.GetString(buf);
				var robj = JsonConvert.DeserializeObject<R>(sjson);
				return robj;
			}

			return default(R);
		}

		private HttpWebRequest CreateRequest(string url, string method, string contentType)
		{
			var request = WebRequest.Create(url) as HttpWebRequest;
			request.Accept = ContentType;
			request.Method = method;
			request.ContentType = contentType;
			return request;
		}
	}
}
