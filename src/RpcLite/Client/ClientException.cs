using System;

namespace RpcLite.Client
{
	/// <summary>
	/// Represents errors that occor during application execution in RpcLite client
	/// </summary>
	public class ClientException : ServiceException
	{
		/// <summary>
		/// Initializes a new instance of RpcLite.ClientClientException class
		/// </summary>
		public ClientException() { }

		/// <summary>
		/// Initializes a new instance of RpcLite.ClientClientException class with specifid message and inner exception
		/// </summary>
		/// <param name="message"></param>
		/// <param name="innerException"></param>
		public ClientException(string message, Exception innerException)
			: base(message, innerException)
		{ }

		/// <summary>
		/// Initializes a new instance of RpcLite.ClientClientException class with specifid message
		/// </summary>
		/// <param name="message">the message that describes the error</param>
		public ClientException(string message)
			: base(message)
		{ }
	}
}
