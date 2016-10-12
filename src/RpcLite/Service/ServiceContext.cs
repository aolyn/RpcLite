using System;
using System.Collections.Generic;
using RpcLite.Formatters;
using RpcLite.Monitor;

namespace RpcLite.Service
{

	/// <summary>
	/// 
	/// </summary>
	public class ServiceContext
	{

		/// <summary>
		/// 
		/// </summary>
		public object ServiceContainer { get; set; }

		/// <summary>
		/// 
		/// </summary>
		public object State { get; set; }

		/// <summary>
		/// 
		/// </summary>
		public RpcAction Action { get; set; }

		/// <summary>
		/// 
		/// </summary>
		public ServiceResponse Response { get; set; }

		/// <summary>
		/// 
		/// </summary>
		public object Result { get; set; }

		/// <summary>
		/// 
		/// </summary>
		public object Tag { get; set; }

		/// <summary>
		/// 
		/// </summary>
		public ServiceRequest Request { get; set; }

		/// <summary>
		/// 
		/// </summary>
		public object Argument { get; set; }

		/// <summary>
		/// 
		/// </summary>
		public RpcService Service { get; set; }

		/// <summary>
		/// 
		/// </summary>
		public IFormatter Formatter { get; set; }

		/// <summary>
		/// 
		/// </summary>
		public object ExtraData { get; set; }

		/// <summary>
		/// 
		/// </summary>
		public IServerContext ExecutingContext { get; set; }

		/// <summary>
		/// 
		/// </summary>
		public Exception Exception { get; set; }

		/// <summary>
		/// 
		/// </summary>
		public IServiceMonitorSession Monitor { get; internal set; }

		/// <summary>
		/// store some use data
		/// </summary>
		private Dictionary<string, object> _extensionData;

		/// <summary>
		/// set extension data by key
		/// </summary>
		/// <param name="key"></param>
		/// <param name="data"></param>
		public void SetExtensionData(string key, object data)
		{
			if (_extensionData == null)
				_extensionData = new Dictionary<string, object>();

			_extensionData[key] = data;
		}

		/// <summary>
		/// get extension data by key
		/// </summary>
		/// <param name="key"></param>
		/// <returns></returns>
		public object GetExtensionData(string key)
		{
			if (_extensionData == null)
				return null;

			object data;
			_extensionData.TryGetValue(key, out data);
			return data;
		}

	}
}
