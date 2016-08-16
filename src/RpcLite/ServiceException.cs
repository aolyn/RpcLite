using System;

#if !NETCORE
using System.Runtime.Serialization;
#endif

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

#if NETCORE
#else
		/// <summary>
		/// 
		/// </summary>
		/// <param name="info"></param>
		/// <param name="context"></param>
		protected ServiceException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}
#endif

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

#if !NETCORE
	///// <summary>
		///// 
		///// </summary>
		///// <param name="info"></param>
		///// <param name="context"></param>
		//protected ActionNotFoundException(SerializationInfo info, StreamingContext context) : base(info, context)
		//{
		//}
#endif

		/// <summary>
		/// Initializes a new instance of RpcLite.ServiceException with specifid message and inner exception
		/// </summary>
		public ActionNotFoundException()
		{ }

		/// <summary>
		/// Initializes a new instance of RpcLite.ServiceException with specifid message and inner exception
		/// </summary>
		/// <param name="actionName">message</param>
		public ActionNotFoundException(string actionName)
			: base($"Action {actionName} Not Found")
		{ }

	}

	/// <summary>
	/// Represents errors that occor during application execution in RpcLite server
	/// </summary>
	public class ServiceNotFoundException : ServiceException
	{
		/// <summary>
		/// Initializes a new instance of RpcLite.ServiceException with specifid message and inner exception
		/// </summary>
		/// <param name="serviceName">message</param>
		/// <param name="innerException">inner exception</param>
		public ServiceNotFoundException(string serviceName, Exception innerException)
			: base($"Service {serviceName} Not Found", innerException)
		{ }

#if !NETCORE
		/// <summary>
		/// 
		/// </summary>
		/// <param name="info"></param>
		/// <param name="context"></param>
		protected ServiceNotFoundException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}
#endif

		/// <summary>
		/// Initializes a new instance of RpcLite.ServiceException with specifid message and inner exception
		/// </summary>
		public ServiceNotFoundException()
		{ }

		/// <summary>
		/// Initializes a new instance of RpcLite.ServiceException with specifid message and inner exception
		/// </summary>
		/// <param name="serviceName">message</param>
		public ServiceNotFoundException(string serviceName)
			: base($"Service {serviceName} Not Found")
		{ }

	}

}
