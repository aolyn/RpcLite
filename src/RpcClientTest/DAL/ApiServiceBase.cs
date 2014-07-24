using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;

namespace WebApiClient.DAL
{
	class ApiServiceBase
	{
		protected HttpClient Client { get; set; }
		protected string RequestUrl { get; set; }

		public ApiServiceBase(string url, string apiUrlPath, string contentTypeHeader)
		{
			RequestUrl = apiUrlPath;

			Client = new HttpClient { BaseAddress = new Uri(url) };
			// AddProduct an Accept header for JSON format.
			// 为JSON格式添加一个Accept报头
			if (!string.IsNullOrWhiteSpace(contentTypeHeader))
				Client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue(contentTypeHeader));
		}

		protected TR GetResult<TR>()
		{
			return GetResult<TR>(RequestUrl);
		}

		private TR GetResult<TR>(string url)
		{
			var response = Client.GetAsync(url).Result;  // Blocking call（阻塞调用）! 
			if (response.IsSuccessStatusCode)
			{
				var result = response.Content.ReadAsAsync<TR>().Result;
				return result;
			}
			throw new Exception("invoke api error");
		}

		protected TR GetResult<TP, TR>(TP arg)
		{
			return GetResult<TR>(RequestUrl + "/" + arg);
		}

		protected TR PostResult<TP, TR>(TP arg)
		{
			var response = Client.PostAsJsonAsync(RequestUrl, arg).Result;
			if (response.IsSuccessStatusCode)
			{
				var rv = response.Content.ReadAsAsync<TR>().Result;
				return rv;
			}
			throw new Exception("error status code");
		}

		protected void PostResult<TP>(TP arg)
		{
			var response = Client.PostAsJsonAsync(RequestUrl, arg).Result;
			if (!response.IsSuccessStatusCode)
			{
				throw new Exception("error status code");
			}
		}
	}
}
