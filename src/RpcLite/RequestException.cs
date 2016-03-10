using System;
using System.Runtime.Serialization;

namespace RpcLite
{
	/// <summary>
	/// Respresnts request error that occor during application in server side
	/// </summary>
	public class RequestException : ServiceException
	{
		/// <summary>
		/// Initializes a new instance of RpcLite.RequestException class
		/// </summary>
		public RequestException() { }

		/// <summary>
		/// 
		/// </summary>
		/// <param name="info"></param>
		/// <param name="context"></param>
		protected RequestException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}

		/// <summary>
		/// Initializes a new instance of RpcLite.RequestException class with specifid message and inner exception
		/// </summary>
		/// <param name="message">message</param>
		/// <param name="innerException">inner exception</param>
		public RequestException(string message, Exception innerException)
			: base(message, innerException)
		{ }

		/// <summary>
		/// Initializes a new instance of RpcLite.RequestException class with specifid message
		/// </summary>
		/// <param name="message">message</param>
		public RequestException(string message)
			: base(message)
		{ }
	}
}
