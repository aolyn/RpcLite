using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Newtonsoft.Json;
using RpcLite.Formatters;
using RpcLite.Net;

namespace RpcLite.Client
{
	/// <summary>
	/// 
	/// </summary>
	/// <typeparam name="T"></typeparam>
	public class RpcClientBase<T> where T : class
	{
		/// <summary>
		/// 
		/// </summary>
		public string BaseUrl { get; set; }

		private IFormatter _formatter = new JsonFormatter();
		/// <summary>
		/// 
		/// </summary>
		public IFormatter Formatter { get { return _formatter; } set { _formatter = value; } }

		/// <summary>
		/// 
		/// </summary>
		/// <param name="action"></param>
		/// <param name="request"></param>
		/// <param name="returnType"></param>
		/// <returns></returns>
		protected object GetResponse(string action, object request, Type returnType)
		{
			var response = DoRequest(action, request, returnType);

			return response;
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="action"></param>
		/// <param name="request"></param>
		/// <param name="returnType"></param>
		/// <returns></returns>
		protected object BeginGetResponse(string action, object request, Type returnType)
		{
			if (_formatter == null)
				throw new ServiceException("Formatter can't be null");

			var mime = _formatter.SupportMimes.First();

			var headDic = new Dictionary<string, string>
				{
					{"Content-Type",mime},
					{"Accept",mime},
				};

			var json = JsonConvert.SerializeObject(request);

			var url = BaseUrl + action;
			var result = WebRequestHelper.PostData(url, json, Encoding.UTF8, headDic);
			if (string.IsNullOrEmpty(result) || returnType == null)
				return null;

			var resultObj = JsonConvert.DeserializeObject(result, returnType);
			return resultObj;
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="action"></param>
		/// <param name="request"></param>
		/// <param name="returnType"></param>
		/// <returns></returns>
		protected object EndGetResponse(string action, object request, Type returnType)
		{
			throw new NotImplementedException();
		}

		/// <summary>
		/// 
		/// </summary>
		public T Client
		{
			get { return this as T; }
		}

		private object DoRequest(string action, object param, Type returnType)
		{
			if (_formatter == null)
				throw new ServiceException("Formatter can't be null");

			var mime = _formatter.SupportMimes.First();

			var resultObj = DoRequest(action, param, returnType, mime);
			return resultObj;
		}

		private object DoRequest(string action, object param, Type returnType, string mime)
		{
			var headDic = new Dictionary<string, string>
				{
					{"Content-Type",mime},
					{"Accept",mime},
				};

			var json = JsonConvert.SerializeObject(param);

			var url = BaseUrl + action;
			var result = WebRequestHelper.PostData(url, json, Encoding.UTF8, headDic);
			if (string.IsNullOrEmpty(result) || returnType == null)
				return null;

			var resultObj = JsonConvert.DeserializeObject(result, returnType);
			return resultObj;
		}

		private static Lazy<Func<RpcClientBase<T>>> _func = new Lazy<Func<RpcClientBase<T>>>(() =>
		{
			var type = ClientWrapper.WrapInterface<T>();
			var func = TypeCreator.GetCreateInstanceFunc(type) as Func<RpcClientBase<T>>;
			return func;
		}, true);

		/// <summary>
		/// 
		/// </summary>
		/// <returns></returns>
		public static RpcClientBase<T> GetInstance()
		{
			return GetInstance(GetDefaultBaseUrl());
		}

		private static string GetDefaultBaseUrl()
		{
			var uri = ClientAddressResolver<T>.GetAddress();
			return uri == null ? null : uri.ToString();
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="baseUrl"></param>
		/// <returns></returns>
		public static RpcClientBase<T> GetInstance(string baseUrl)
		{
			if (_func.Value == null)
				throw new ClientException("GetCreateInstanceFunc Error.");

			var client = _func.Value();
			client.BaseUrl = baseUrl;
			return client;
		}
	}
}
