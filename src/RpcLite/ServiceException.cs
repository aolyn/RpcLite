using System;

namespace RpcLite
{
	/// <summary>
	/// Represents errors that occor during application execution in RpcLite server
	/// </summary>
	public class ServiceException : RpcLiteException
	{
		/// <summary>
		/// Initializes a new instance of RpcLite.ServiceException class
		/// </summary>
		public ServiceException() { }

		/// <summary>
		/// Initializes a new instance of RpcLite.ServiceException with specifid message and inner exception
		/// </summary>
		/// <param name="message">message</param>
		/// <param name="innerException">inner exception</param>
		public ServiceException(string message, Exception innerException)
			: base(message, innerException)
		{ }

		/// <summary>
		/// Initializes a new instance of RpcLite.ServiceException with specifid message
		/// </summary>
		/// <param name="message"></param>
		public ServiceException(string message)
			: base(message)
		{ }
	}

	/// <summary>
	/// Represents errors that occor during application execution in RpcLite server
	/// </summary>
	public class ActionNotFoundException : ServiceException
	{

		/// <summary>
		/// Initializes a new instance of RpcLite.ServiceException with specifid message and inner exception
		/// </summary>
		/// <param name="actionName">message</param>
		/// <param name="innerException">inner exception</param>
		public ActionNotFoundException(string actionName, Exception innerException)
			: base($"Action {actionName} Not Found", innerException)
		{ }

		/// <summary>
		/// Initializes a new instance of RpcLite.ServiceException with specifid message and inner exception
		/// </summary>
		/// <param name="actionName">message</param>
		public ActionNotFoundException(string actionName)
			: base($"Action {actionName} Not Found")
		{ }

	}

}
