////#define LogDuration

//#if DEBUG && LogDuration
//using System.Diagnostics;
//#endif

//using System;
//using System.Collections.Generic;
//using System.IO;
//using System.Net;
//using System.Text;
//using System.Threading.Tasks;
//using RpcLite.Client;

//namespace RpcLite.Net
//{
//	/// <summary>
//	/// Contain simple methods to process web request &amp; response
//	/// </summary>
//	public partial class WebRequestHelper
//	{
//		private const string HeadPrefix = "RpcLite-";

//		///// <summary>
//		///// post data to web server and get retrieve response data
//		///// </summary>
//		///// <param name="url"></param>
//		///// <param name="postData"></param>
//		///// <param name="encoding"></param>
//		///// <param name="headDic"></param>
//		///// <returns></returns>
//		//public static string PostData(string url, string postData, Encoding encoding, Dictionary<string, string> headDic)
//		//{
//		//	var response = Post(url, postData, encoding, headDic);
//		//	return response.Result;
//		//}

//#if NETFX_40
//		private int a2324234 = 2;
//#endif

//		/// <summary>
//		/// post data to web server and get retrieve response data
//		/// </summary>
//		/// <param name="url"></param>
//		/// <param name="postData"></param>
//		/// <param name="encoding"></param>
//		/// <param name="headDic"></param>
//		/// <returns></returns>
//		public static Task<ServiceReponseMessage> PostAsync(string url, string postData, Encoding encoding, Dictionary<string, string> headDic)
//		{
//			var tcs = new TaskCompletionSource<ServiceReponseMessage>();

//			var request = (HttpWebRequest)WebRequest.Create(url);
//			//request.Proxy = new WebProxy("http://localhost:8888/");
//			request.Method = "POST";
//			if (headDic != null)
//			{
//				foreach (var head in headDic)
//				{
//					if (head.Key == "Content-Type")
//						request.ContentType = head.Value;
//					else if (head.Key == "Accept")
//						request.Accept = head.Value;
//					else
//						request.Headers[head.Key] = head.Value;
//				}
//			}

//			Action setp2 = () =>
//			{
//				var getResponseTask = Task.Factory.FromAsync(request.BeginGetResponse, request.EndGetResponse, null);

//				// ReSharper disable once UnusedVariable
//				var task1 = getResponseTask.ContinueWith(tsk =>
//				{
//					ServiceReponseMessage responseMessage;

//					if (tsk.Exception != null)
//					{
//						var webException = tsk.Exception.InnerException as WebException;
//						if (webException == null)
//						{
//							tcs.SetException(tsk.Exception.InnerException);
//							return;
//						}

//						if (webException.Response != null)
//						{
//							try
//							{
//								responseMessage = GetResponseMessage(encoding, (HttpWebResponse)webException.Response);
//								tcs.SetResult(responseMessage);
//							}
//							catch (Exception ex)
//							{
//								tcs.SetException(ex);
//							}
//							finally
//							{
//								((IDisposable)webException.Response).Dispose();
//							}
//						}
//						else
//						{
//							tcs.SetException(new ConnectionException("connection error when transport data with server", webException));
//						}

//						return;
//					}

//					try
//					{
//						using (var response = (HttpWebResponse)getResponseTask.Result)
//						{
//							responseMessage = GetResponseMessage(encoding, response);
//						}
//					}
//					catch (WebException ex)
//					{
//						if (ex.Response != null)
//						{
//							responseMessage = GetResponseMessage(encoding, (HttpWebResponse)ex.Response);
//							((IDisposable)ex.Response).Dispose();
//						}
//						else
//						{
//							tcs.SetException(ex);
//							return;
//						}
//					}
//					tcs.SetResult(responseMessage);
//				});
//			};

//			if (!string.IsNullOrEmpty(postData))
//			{
//				var getRequestStreamTask = Task.Factory.FromAsync(request.BeginGetRequestStream, request.EndGetRequestStream, null);
//				// ReSharper disable once UnusedVariable
//				var task1 = getRequestStreamTask.ContinueWith(tsk =>
//				{
//					if (tsk.Exception != null)
//					{
//						tcs.SetException(tsk.Exception);
//						return;
//					}

//					var requestStream = tsk.Result;

//					if (encoding == null)
//						encoding = Encoding.UTF8;

//					var bytes = encoding.GetBytes(postData);
//					try
//					{
//						requestStream.Write(bytes, 0, bytes.Length);
//						requestStream.Dispose();
//					}
//					catch (Exception ex)
//					{
//						tcs.SetException(ex);
//						return;
//					}

//					setp2();
//				});
//			}
//			else
//			{
//				setp2();
//			}

//			return tcs.Task;
//		}

//		/// <summary>
//		/// post data to web server and get retrieve response data
//		/// </summary>
//		/// <param name="url"></param>
//		/// <param name="content"></param>
//		/// <param name="headDic"></param>
//		/// <returns></returns>
//		public static Task<ResponseMessage> PostAsync(string url, IContent content, IDictionary<string, string> headDic)
//		{
//			var tcs = new TaskCompletionSource<ResponseMessage>();

//			var request = (HttpWebRequest)WebRequest.Create(url);
//			//request.Proxy = new WebProxy("http://localhost:8888/");
//			request.Method = "POST";
//			if (headDic != null)
//			{
//				foreach (var head in headDic)
//				{
//					if (head.Key == "Content-Type")
//						request.ContentType = head.Value;
//					else if (head.Key == "Accept")
//						request.Accept = head.Value;
//					else
//						request.Headers[head.Key] = head.Value;
//				}
//			}

//			Action setp2 = () =>
//			{
//#if DEBUG && LogDuration
//						var stopwatch1 = Stopwatch.StartNew();

//						//#if NETCORE
//						//				var response1 = request.GetResponseAsync().Result;
//						//				var duration0 = stopwatch1.GetAndRest();
//						//#endif
//#endif
//				var getResponseTask = Task.Factory.FromAsync(request.BeginGetResponse, request.EndGetResponse, null);

//				// ReSharper disable once UnusedVariable
//				var task1 = getResponseTask.ContinueWith(tsk =>
//				{
//#if DEBUG && LogDuration
//							var duration1 = stopwatch1.GetAndRest();
//#endif

//					ResponseMessage responseMessage;
//					if (tsk.Exception != null)
//					{
//						var webException = tsk.Exception.InnerException as WebException;
//						if (webException == null)
//						{
//							tcs.SetException(tsk.Exception.InnerException);
//							return;
//						}

//						if (webException.Response != null)
//						{
//							try
//							{
//								responseMessage = GetResponseMessage((HttpWebResponse)webException.Response);
//								tcs.SetResult(responseMessage);
//							}
//							catch (Exception ex)
//							{
//								tcs.SetException(ex);
//							}
//							//finally
//							//{
//							//	((IDisposable)webException.Response).Dispose();
//							//}
//						}
//						else
//						{
//							tcs.SetException(new ConnectionException("connection error when transport data with server", webException));
//						}

//						return;
//					}

//					try
//					{
//						//using (var response = (HttpWebResponse)getResponseTask.Result)
//						//{
//						//	responseMessage = GetResponseMessage(response);
//						//}
//						responseMessage = GetResponseMessage((HttpWebResponse)getResponseTask.Result);
//					}
//					catch (WebException ex)
//					{
//						if (ex.Response != null)
//						{
//							responseMessage = GetResponseMessage((HttpWebResponse)ex.Response);
//							((IDisposable)ex.Response).Dispose();
//						}
//						else
//						{
//							tcs.SetException(ex);
//							return;
//						}
//					}
//					tcs.SetResult(responseMessage);
//				});
//			};

//			if (content != null)
//			{
//#if DEBUG && LogDuration
//						var stopwatch1 = Stopwatch.StartNew();
//#endif

//				var getRequestStreamTask = Task.Factory.FromAsync(request.BeginGetRequestStream, request.EndGetRequestStream, null);
//				// ReSharper disable once UnusedVariable
//				var task1 = getRequestStreamTask.ContinueWith(tsk =>
//				{
//#if DEBUG && LogDuration
//							var duration0 = stopwatch1.GetAndRest();
//#endif

//					if (tsk.Exception != null)
//					{
//						tcs.SetException(tsk.Exception);
//						return;
//					}

//					var requestStream = tsk.Result;

//					//var bytes = postData;
//					try
//					{
//						content.Write(requestStream);
//						//requestStream.Write(bytes, 0, bytes.Length);
//						requestStream.Dispose();

//#if DEBUG && LogDuration
//								var duration1 = stopwatch1.GetAndRest();
//#endif
//					}
//					catch (Exception ex)
//					{
//						tcs.SetException(ex);
//						return;
//					}

//					setp2();
//				});
//			}
//			else
//			{
//				setp2();
//			}

//			return tcs.Task;
//		}

//		/// <summary>
//		/// post data to web server and get retrieve response data
//		/// </summary>
//		/// <param name="url"></param>
//		/// <param name="postData"></param>
//		/// <param name="encoding"></param>
//		/// <param name="headDic"></param>
//		/// <returns></returns>
//		public static ServiceReponseMessage Post(string url, string postData, Encoding encoding, Dictionary<string, string> headDic)
//		{
//			var task = PostAsync(url, postData, encoding, headDic);
//			try
//			{
//				return task.Result;
//			}
//			catch (AggregateException ex)
//			{
//				throw ex.InnerException;
//			}
//			//if (task.Exception != null)
//			//{
//			//}
//			//return task.Result;

//			#region

//			//			ServiceReponseMessage responseMessage;
//			//			var request = WebRequest.Create(url) as HttpWebRequest;
//			//			if (request == null) return null;

//			//#if NETCORE
//			//			request.ContinueTimeout = 3000 * 1000;
//			//#else
//			//			request.Timeout = 3000 * 1000;
//			//#endif
//			//			request.Method = "POST";
//			//			if (headDic != null)
//			//			{
//			//				foreach (var head in headDic)
//			//				{
//			//					if (head.Key == "Content-Type")
//			//						request.ContentType = head.Value;
//			//					else if (head.Key == "Accept")
//			//						request.Accept = head.Value;
//			//					else
//			//						request.Headers[head.Key] = head.Value;
//			//				}
//			//			}

//			//			if (!string.IsNullOrEmpty(postData))
//			//			{

//			//				var requestStream = request.GetRequestStream();

//			//				if (encoding == null)
//			//					encoding = Encoding.UTF8;
//			//				var writer = new StreamWriter(requestStream, encoding);
//			//				writer.Write(postData);
//			//				writer.Dispose();
//			//			}

//			//			try
//			//			{
//			//				using (var response = (HttpWebResponse)request.GetResponse())
//			//				{
//			//					responseMessage = GetResponseMessage(encoding, response);
//			//				}
//			//			}
//			//			catch (WebException ex)
//			//			{
//			//				if (ex.Response != null)
//			//				{
//			//					responseMessage = GetResponseMessage(encoding, (HttpWebResponse)ex.Response);
//			//					((IDisposable)ex.Response).Dispose();
//			//				}
//			//				else
//			//				{
//			//					throw;
//			//				}
//			//			}
//			//			return responseMessage;

//			#endregion
//		}

//		private static ServiceReponseMessage GetResponseMessage(Encoding encoding, HttpWebResponse response)
//		{
//			var headers = GetRpcHeaders(response.Headers);

//			string jsonResult = null;

//			var contentEncoding = response.Headers[HttpRequestHeader.TransferEncoding];
//			if (response.ContentLength > 0 || contentEncoding == "chunked")
//			{
//				var stream = response.GetResponseStream();
//				if (stream != null)
//				{
//					var reader = new StreamReader(stream, encoding);
//					jsonResult = reader.ReadToEnd();
//				}
//			}

//			string statusCode;
//			var isSuccess = headers.TryGetValue(HeaderName.StatusCode, out statusCode)
//				&& statusCode == ((int)HttpStatusCode.OK).ToString();

//			var responseMessage = new ServiceReponseMessage
//			{
//				IsSuccess = isSuccess,
//				Result = jsonResult,
//				Header = headers,
//			};

//			return responseMessage;
//		}

//		/// <summary>
//		/// 
//		/// </summary>
//		public class ServiceReponseMessage
//		{
//			/// <summary>
//			/// 
//			/// </summary>
//			public bool IsSuccess { get; set; }

//			/// <summary>
//			/// 
//			/// </summary>
//			public Dictionary<string, string> Header { get; set; }

//			/// <summary>
//			/// 
//			/// </summary>
//			public string Result { get; set; }
//		}
//	}
//}