using System;
using System.Collections.Generic;
using System.IO;

namespace RpcLite.Client
{
	/// <summary>
	/// 
	/// </summary>
	public class ResponseMessage : IResponseMessage
	{
		private readonly IDisposable _obj;

		/// <summary>
		/// 
		/// </summary>
		/// <param name="obj"></param>
		public ResponseMessage(IDisposable obj)
		{
			_obj = obj;
		}

		/// <summary>
		/// 
		/// </summary>
		public bool IsSuccess { get; set; }

		/// <summary>
		/// 
		/// </summary>
		public IDictionary<string, string> Headers { get; set; }

		/// <summary>
		/// 
		/// </summary>
		public Stream Result { get; set; }

		/// <summary>
		/// 
		/// </summary>
		public void Dispose()
		{
			_obj?.Dispose();
		}

	}
}
