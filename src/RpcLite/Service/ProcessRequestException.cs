using System;
using System.Runtime.Serialization;

namespace RpcLite.Service
{
	/// <summary>
	/// Respresnts request error that occor during process request in server side
	/// </summary>
	public class ProcessRequestException : ServiceException
	{
		/// <summary>
		/// Initializes a new instance of RpcLite.ProcessRequestException class
		/// </summary>
		public ProcessRequestException() { }

		/// <summary>
		/// 
		/// </summary>
		/// <param name="info"></param>
		/// <param name="context"></param>
		protected ProcessRequestException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}

		/// <summary>
		/// Initializes a new instance of RpcLite.ProcessRequestException class with specifid message and inner exception
		/// </summary>
		/// <param name="message">message</param>
		/// <param name="innerException">inner exception</param>
		public ProcessRequestException(string message, Exception innerException)
			: base(message, innerException)
		{ }

		/// <summary>
		/// Initializes a new instance of RpcLite.ProcessRequestException class with specifid message and inner exception
		/// </summary>
		/// <param name="message">message</param>
		public ProcessRequestException(string message)
			: base(message)
		{ }
	}
}
