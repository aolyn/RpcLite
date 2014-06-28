using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Hrj.Net;
using Newtonsoft.Json;
using RpcLite.Formatters;

namespace RpcLite.Client
{
	public class RpcClientBase<T> where T : class
	{
		public string BaseUrl { get; set; }

		private IFormatter _formatter = new JsonFormatter();
		public IFormatter Formatter { get { return _formatter; } set { _formatter = value; } }

		protected object GetResponse(string action, object request, Type returnType)
		{
			var response = DoRequest(action, request, returnType);

			return response;
		}

		public T Client
		{
			get { return this as T; }
		}

		private object DoRequest(string action, object param, Type returnType)
		{
			if (_formatter == null)
				throw new ServiceException("Formatter can't be null");

			var mime = _formatter.SupportMimes.First();

			var resultObj = GetResponse(action, param, returnType, mime);
			return resultObj;
		}

		private object GetResponse(string action, object param, Type returnType, string mime)
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

		private static Func<RpcClientBase<T>> _func;
		private readonly static object createImplementTypeLock = new object();
		public static RpcClientBase<T> CreateInstance()
		{
			if (_func == null)
			{
				lock (createImplementTypeLock)
				{
					if (_func == null)
					{
						var type = ClientWrapper.WrapInterface<T>();
						_func = TypeCreator.GetCreateInstanceFunc(type) as Func<RpcClientBase<T>>;
					}
				}
			}

			if (_func == null)
				throw new ClientException("GetCreateInstanceFunc Error.");

			return _func();
		}
	}
}
