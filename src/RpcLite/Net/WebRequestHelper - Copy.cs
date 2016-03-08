using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace RpcLite.Net
{
	/// <summary>
	/// Contain simple methods to process web request &amp; response
	/// </summary>
	public class WebRequestHelper
	{
		/// <summary>
		/// post data to web server and get retrieve response data
		/// </summary>
		/// <param name="url"></param>
		/// <param name="postData"></param>
		/// <param name="encoding"></param>
		/// <param name="headDic"></param>
		/// <returns></returns>
		public static string PostData(string url, string postData, Encoding encoding, Dictionary<string, string> headDic)
		{
			var response = Post(url, postData, encoding, headDic);
			return response.Result;
		}

		/// <summary>
		/// post data to web server and get retrieve response data
		/// </summary>
		/// <param name="url"></param>
		/// <param name="postData"></param>
		/// <param name="encoding"></param>
		/// <param name="headDic"></param>
		/// <returns></returns>
		public static Task<WebRequestReponseMessage> PostAsync(string url, string postData, Encoding encoding, Dictionary<string, string> headDic)
		{
			var tcs = new TaskCompletionSource<WebRequestReponseMessage>();

			var client = new HttpClient();

			var content = new StringContent(postData);
			if (headDic != null)
			{
				foreach (var head in headDic)
				{
					if (head.Key == "Content-Type")
						content.Headers.ContentType = new MediaTypeHeaderValue(head.Value);
					else if (head.Key == "Accept")
						continue;
					else
						client.DefaultRequestHeaders.Add(head.Key, head.Value);
				}
			}

			var task = client.PostAsync(url, content);
			var procTask2 = task.ContinueWith(tsk =>
			{
				if (tsk.Exception != null)
				{
					tcs.SetException(tsk.Exception);
				}
				else
				{
					var responseMessage = tsk.Result;
					var readTask = responseMessage.Content.ReadAsStringAsync();
					var procReadTask = readTask.ContinueWith(rtsk =>
					{
						if (rtsk.Exception != null)
						{
							tcs.SetException(rtsk.Exception);
						}
						else
						{

							var headers = responseMessage.Headers
								.Where(it => it.Key.StartsWith("RpcLite-"))
								.ToDictionary(item => item.Key, item => item.Value.FirstOrDefault());

							var resp = rtsk.Result;

							//string jsonResult = null;

							//if (response.ContentLength > 0)
							//{
							//	var stream = response.GetResponseStream();
							//	if (stream != null)
							//	{
							//		var reader = new StreamReader(stream, encoding);
							//		jsonResult = reader.ReadToEnd();
							//	}
							//}

							var re = new WebRequestReponseMessage
							{
								IsSuccess = responseMessage.StatusCode == HttpStatusCode.OK,
								Result = resp,
								Header = headers,
							};
							tcs.SetResult(re);
							//return responseMessage;

						}
					});
				}
			});

			//var procTask = task.ContinueWith(tsk =>
			//{
			//	var response = tsk.Result;
			//	WebRequestReponseMessage responseMessage = null;

			//	try
			//	{
			//		//var rv = response.Content.
			//		//		responseMessage = GetResponseMessage(encoding, response);
			//	}
			//	catch (WebException ex)
			//	{
			//		if (ex.Response != null)
			//		{
			//			responseMessage = GetResponseMessage(encoding, (HttpWebResponse)ex.Response);
			//			ex.Response.Close();
			//		}
			//		else
			//		{
			//			throw;
			//		}
			//	}
			//	return responseMessage;

			//});

			return tcs.Task;
		}

		/// <summary>
		/// post data to web server and get retrieve response data
		/// </summary>
		/// <param name="url"></param>
		/// <param name="postData"></param>
		/// <param name="encoding"></param>
		/// <param name="headDic"></param>
		/// <returns></returns>
		public static WebRequestReponseMessage Post(string url, string postData, Encoding encoding, Dictionary<string, string> headDic)
		{
			WebRequestReponseMessage responseMessage = null;
			var request = WebRequest.Create(url) as HttpWebRequest;
			if (request != null)
			{
				request.Method = "POST";
				if (headDic != null)
				{
					foreach (var head in headDic)
					{
						if (head.Key == "Content-Type")
							request.ContentType = head.Value;
						else if (head.Key == "Accept")
							request.Accept = head.Value;
						else
							request.Headers.Add(head.Key, head.Value);
					}
				}

				if (!string.IsNullOrEmpty(postData))
				{
					var requestStream = request.GetRequestStream();
					if (encoding == null)
						encoding = Encoding.UTF8;
					var writer = new StreamWriter(requestStream, encoding);
					writer.Write(postData);
					writer.Close();
				}

				try
				{
					using (var response = (HttpWebResponse)request.GetResponse())
					{
						responseMessage = GetResponseMessage(encoding, response);
					}
				}
				catch (WebException ex)
				{
					if (ex.Response != null)
					{
						responseMessage = GetResponseMessage(encoding, (HttpWebResponse)ex.Response);
						ex.Response.Close();
					}
					else
					{
						throw;
					}
				}
			}
			return responseMessage;
		}

		private static WebRequestReponseMessage GetResponseMessage(Encoding encoding, HttpWebResponse response)
		{
			var headers = response.Headers
				.Cast<string>()
				.Where(it => it.StartsWith("RpcLite-"))
				.ToDictionary(item => item, item => response.Headers[item]);

			string jsonResult = null;

			if (response.ContentLength > 0)
			{
				var stream = response.GetResponseStream();
				if (stream != null)
				{
					var reader = new StreamReader(stream, encoding);
					jsonResult = reader.ReadToEnd();
				}
			}

			var responseMessage = new WebRequestReponseMessage
			{
				IsSuccess = response.StatusCode == HttpStatusCode.OK,
				Result = jsonResult,
				Header = headers,
			};

			return responseMessage;
		}

		/// <summary>
		/// 
		/// </summary>
		public class WebRequestReponseMessage
		{
			/// <summary>
			/// 
			/// </summary>
			public bool IsSuccess { get; set; }
			/// <summary>
			/// 
			/// </summary>
			public Dictionary<string, string> Header { get; set; }
			/// <summary>
			/// 
			/// </summary>
			public string Result { get; set; }
		}
	}
}
