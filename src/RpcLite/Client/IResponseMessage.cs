using System;
using System.Collections.Generic;
using System.IO;

namespace RpcLite.Client
{
	/// <summary>
	/// 
	/// </summary>
	public interface IResponseMessage : IDisposable
	{
		/// <summary>
		/// 
		/// </summary>
		bool IsSuccess { get; set; }

		/// <summary>
		/// 
		/// </summary>
		IDictionary<string, string> Headers { get; set; }

		/// <summary>
		/// 
		/// </summary>
		Stream Result { get; set; }
	}
}
