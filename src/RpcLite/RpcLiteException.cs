using System;

namespace RpcLite
{
	/// <summary>
	/// Represents errors that occor during application execution in RpcLite server
	/// </summary>
	public class RpcLiteException : Exception
	{
		/// <summary>
		/// Initializes a new instance of RpcLite.ServiceException class
		/// </summary>
		public RpcLiteException() { }

		/// <summary>
		/// Initializes a new instance of RpcLite.ServiceException with specifid message and inner exception
		/// </summary>
		/// <param name="message">message</param>
		/// <param name="innerException">inner exception</param>
		public RpcLiteException(string message, Exception innerException)
			: base(message, innerException)
		{ }

		/// <summary>
		/// Initializes a new instance of RpcLite.ServiceException with specifid message
		/// </summary>
		/// <param name="message"></param>
		public RpcLiteException(string message)
			: base(message)
		{ }
	}
}
