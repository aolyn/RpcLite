using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using RpcLite.Client;

namespace RpcLite.Net
{
	internal static class WebRequestHelper
	{
		private const string HeadPrefix = "RpcLite-";

		private static ResponseMessage GetResponseMessage(HttpWebResponse response)
		{
			var headers = GetRpcHeaders(response.Headers);

			var isSuccess = headers.TryGetValue(HeaderName.StatusCode, out var statusCode)
				&& statusCode == RpcStatusCode.Ok;

			var responseMessage = new ResponseMessage(response)
			{
				IsSuccess = isSuccess,
				Result = response.GetResponseStream(),
				Headers = headers,
			};
			return responseMessage;
		}

		private static Dictionary<string, string> GetRpcHeaders(WebHeaderCollection headers)
		{
			return headers
				.Cast<string>()
				.Where(it => it.StartsWith(HeadPrefix))
				.ToDictionary(item => item.Substring(HeadPrefix.Length, item.Length - HeadPrefix.Length), item => headers[item]);
		}

		private static Dictionary<string, string> GetRpcHeaders(HttpResponseHeaders headers)
		{
			return headers
				.Where(it => it.Key.StartsWith(HeadPrefix))
				.ToDictionary(item => item.Key.Substring(HeadPrefix.Length, item.Key.Length - HeadPrefix.Length), item => item.Value.FirstOrDefault());
		}

		/// <summary>
		/// post data to web server and get retrieve response data
		/// </summary>
		/// <param name="httpClient"></param>
		/// <param name="url"></param>
		/// <param name="content"></param>
		/// <param name="headDic"></param>
		/// <returns></returns>
		public static Task<IResponseMessage> PostAsync(HttpClient httpClient, string url, Stream content,
			IDictionary<string, string> headDic)
		{
			var requestMessage = new HttpRequestMessage(HttpMethod.Post, url)
			{
				Content = new StreamContent(content)
			};

			if (headDic != null && headDic.Count > 0)
			{
				if (headDic.TryGetValue("Content-Type", out var contentType))
				{
					requestMessage.Content.Headers.ContentType = new MediaTypeHeaderValue(contentType);
					//requestMessage.Headers.TryAddWithoutValidation("Content-Type", contentType);
				}
			}

#if DEBUG && LogDuration
			var stopwatch1 = Stopwatch.StartNew();
#endif

			var responseTask = httpClient.SendAsync(requestMessage);
			return responseTask.ContinueWith(tsk =>
			{
#if DEBUG && LogDuration
					var duration1 = stopwatch1.GetAndRest();
#endif

				IResponseMessage responseMessage;
				if (tsk.Exception != null)
				{
					var webException = tsk.Exception.InnerException as HttpRequestException;
					if (webException == null)
					{
						throw tsk.Exception.InnerException;
					}

					//TODO check error like 500, 403
					//if (webException.Response != null)
					//{
					//	try
					//	{
					//		responseMessage = GetResponseMessage((HttpWebResponse)webException.Response);
					//		return responseMessage;
					//	}
					//	catch (Exception ex)
					//	{
					//		LogHelper.Error(ex);
					//		throw;
					//	}
					//}
					throw new ConnectionException("connection error when transport data with server", webException);
				}

				if (!tsk.Result.IsSuccessStatusCode)
				{
					throw new ConnectionException("connection error when transport data with server: " + tsk.Result.ReasonPhrase);
				}

				try
				{
					responseMessage = GetResponseMessage(tsk.Result);
				}
				catch (WebException ex)
				{
					if (ex.Response != null)
					{
						responseMessage = GetResponseMessage((HttpWebResponse)ex.Response);
						((IDisposable)ex.Response).Dispose();
					}
					else
					{
						throw;
					}
				}
				return responseMessage;

				//#if DEBUG && LogDuration
				//	var duration1 = stopwatch1.GetAndRest();
				//#endif
				//	var responseMessage = GetResponseMessage(tsk.Result);
				//#if DEBUG && LogDuration
				//	var duration2 = stopwatch1.GetAndRest();
				//#endif
				//	return responseMessage;
			});
		}

		private static IResponseMessage GetResponseMessage(HttpResponseMessage response)
		{
			var headers = GetRpcHeaders(response.Headers);
			//var contentEncoding = response.Headers.GetValues("Transfer-Encoding").FirstOrDefault();
			string statusCode;
			var isSuccess = headers.TryGetValue(HeaderName.StatusCode, out statusCode)
				&& statusCode == RpcStatusCode.Ok;

			var responseMessage = new ResponseMessage(response)
			{
				IsSuccess = isSuccess, // response.StatusCode == HttpStatusCode.OK,
				Result = response.Content.ReadAsStreamAsync().Result,
				Headers = headers,
			};
			return responseMessage;
		}
	}
}
