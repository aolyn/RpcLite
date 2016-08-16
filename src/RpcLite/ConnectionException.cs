using System;
#if !NETCORE
using System.Runtime.Serialization;
#endif

namespace RpcLite
{
	/// <summary>
	/// Represents errors that occor during connect to server in client
	/// </summary>
	public class ConnectionException : RpcLiteException
	{
		/// <summary>
		/// Initializes a new instance of RpcLite.ConnectException class
		/// </summary>
		public ConnectionException() { }

#if NETCORE
#else
		/// <summary>
		/// 
		/// </summary>
		/// <param name="info"></param>
		/// <param name="context"></param>
		protected ConnectionException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}
#endif

		/// <summary>
		/// Initializes a new instance of RpcLite.ConnectException with specifid message and inner exception
		/// </summary>
		/// <param name="message">message</param>
		/// <param name="innerException">inner exception</param>
		public ConnectionException(string message, Exception innerException)
			: base(message, innerException)
		{ }

		/// <summary>
		/// Initializes a new instance of RpcLite.ConnectException with specifid message
		/// </summary>
		/// <param name="message"></param>
		public ConnectionException(string message)
			: base(message)
		{ }
	}

}
