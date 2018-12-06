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
		private const string RequestMarkPropertyKey = "RpcLiteClient-HttpRequest";

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
				.ToDictionary(item => item.Substring(HeadPrefix.Length, item.Length - HeadPrefix.Length),
					item => headers[item]);
		}

		private static Dictionary<string, string> GetRpcHeaders(HttpResponseHeaders headers)
		{
			return headers
				.Where(it => it.Key.StartsWith(HeadPrefix))
				.ToDictionary(item => item.Key.Substring(HeadPrefix.Length, item.Key.Length - HeadPrefix.Length),
					item => item.Value.FirstOrDefault());
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
			requestMessage.Properties[RequestMarkPropertyKey] = "";
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
			return responseTask.ContinueWithTask(tsk =>
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
						throw tsk.Exception?.InnerException ?? tsk.Exception;
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
					throw new ConnectionException("connection error when transport data with server: "
					    + tsk.Result.ReasonPhrase);
				}

				return GetResponseMessage(tsk.Result).ContinueWith(msgTask =>
				{
					if (msgTask.Exception == null)
						return msgTask.Result;

					var ex = msgTask.Exception.InnerException;
					if (!(ex is WebException webException))
					{
						// ReSharper disable once PossibleNullReferenceException
						throw ex;
					}

					if (webException.Response == null) throw ex;

					responseMessage = GetResponseMessage((HttpWebResponse)webException.Response);
					((IDisposable)webException.Response).Dispose();
					return responseMessage;
				});
			});
		}

		private static Task<IResponseMessage> GetResponseMessage(HttpResponseMessage response)
		{
			var headers = GetRpcHeaders(response.Headers);
			//var contentEncoding = response.Headers.GetValues("Transfer-Encoding").FirstOrDefault();
			var isSuccess = headers.TryGetValue(HeaderName.StatusCode, out var statusCode)
				&& statusCode == RpcStatusCode.Ok;

			var resultTask = response.Content.ReadAsStreamAsync().ContinueWith(tsk =>
			{
				if (tsk.Exception != null)
				{
					// ReSharper disable once PossibleNullReferenceException
					throw tsk.Exception.InnerException;
				}

				var responseMessage = (IResponseMessage)new ResponseMessage(response)
				{
					IsSuccess = isSuccess,
					Result = tsk.Result,
					Headers = headers,
				};
				return responseMessage;
			});
			return resultTask;
		}
	}
}