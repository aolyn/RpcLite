using System;

#if !NETCORE
using System.Runtime.Serialization;
#endif

namespace RpcLite
{
	/// <summary>
	/// Represents errors that occor during application execution in RpcLite server
	/// </summary>
#if NETCORE
#else
	[Serializable]
#endif
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

	/// <summary>
	/// Represents errors that occor during application execution in RpcLite server
	/// </summary>
	public class NotInitializedException : RpcLiteException
	{
		/// <summary>
		/// Initializes a new instance of RpcLite.RpcNotInitializedException class
		/// </summary>
		public NotInitializedException() { }

#if NETCORE
#else
		/// <summary>
		/// 
		/// </summary>
		/// <param name="info"></param>
		/// <param name="context"></param>
		protected NotInitializedException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}
#endif

		/// <summary>
		/// Initializes a new instance of RpcLite.RpcNotInitializedException with specifid message and inner exception
		/// </summary>
		/// <param name="message">message</param>
		/// <param name="innerException">inner exception</param>
		public NotInitializedException(string message, Exception innerException)
			: base(message, innerException)
		{ }

		/// <summary>
		/// Initializes a new instance of RpcLite.RpcNotInitializedException with specifid message
		/// </summary>
		/// <param name="message"></param>
		public NotInitializedException(string message)
			: base(message)
		{ }
	}


	/// <summary>
	/// Represents errors that occor during application execution in RpcLite server
	/// </summary>
	public class DeserializeRequestException : ServiceException
	{
		/// <summary>
		/// Initializes a new instance of RpcLite.ServiceException with specifid message and inner exception
		/// </summary>
		/// <param name="message">message</param>
		/// <param name="innerException">inner exception</param>
		public DeserializeRequestException(string message, Exception innerException)
			: base(message, innerException)
		{ }

#if !NETCORE
	///// <summary>
		///// 
		///// </summary>
		///// <param name="info"></param>
		///// <param name="context"></param>
		//protected DeserializeRequestException(SerializationInfo info, StreamingContext context) : base(info, context)
		//{
		//}
#endif

		/// <summary>
		/// Initializes a new instance of RpcLite.ServiceException with specifid message and inner exception
		/// </summary>
		public DeserializeRequestException()
		{ }

		/// <summary>
		/// Initializes a new instance of RpcLite.ServiceException with specifid message and inner exception
		/// </summary>
		/// <param name="actionName">message</param>
		public DeserializeRequestException(string actionName)
			: base($"Action {actionName} Not Found")
		{ }

	}


	/// <summary>
	/// Represents errors that occor during application execution in RpcLite server
	/// </summary>
	public class SerializeRequestException : ServiceException
	{
		/// <summary>
		/// Initializes a new instance of RpcLite.ServiceException with specifid message and inner exception
		/// </summary>
		/// <param name="message">message</param>
		/// <param name="innerException">inner exception</param>
		public SerializeRequestException(string message, Exception innerException)
			: base(message, innerException)
		{ }

#if !NETCORE
	///// <summary>
		///// 
		///// </summary>
		///// <param name="info"></param>
		///// <param name="context"></param>
		//protected SerializeRequestException(SerializationInfo info, StreamingContext context) : base(info, context)
		//{
		//}
#endif

		/// <summary>
		/// Initializes a new instance of RpcLite.ServiceException with specifid message and inner exception
		/// </summary>
		public SerializeRequestException()
		{ }

		/// <summary>
		/// Initializes a new instance of RpcLite.ServiceException with specifid message and inner exception
		/// </summary>
		/// <param name="actionName">message</param>
		public SerializeRequestException(string actionName)
			: base($"Action {actionName} Not Found")
		{ }

	}

	/// <summary>
	/// Represents errors that occor during application execution in RpcLite server
	/// </summary>
	public class FormatterNotFoundException : ServiceException
	{
		/// <summary>
		/// Initializes a new instance of RpcLite.ServiceException with specifid message and inner exception
		/// </summary>
		/// <param name="name">message</param>
		/// <param name="innerException">inner exception</param>
		public FormatterNotFoundException(string name, Exception innerException)
			: base(name, innerException)
		{ }

#if !NETCORE
	///// <summary>
		///// 
		///// </summary>
		///// <param name="info"></param>
		///// <param name="context"></param>
		//protected FormatterNotFoundException(SerializationInfo info, StreamingContext context) : base(info, context)
		//{
		//}
#endif

		/// <summary>
		/// Initializes a new instance of RpcLite.ServiceException with specifid message and inner exception
		/// </summary>
		public FormatterNotFoundException()
		{ }

		/// <summary>
		/// Initializes a new instance of RpcLite.ServiceException with specifid message and inner exception
		/// </summary>
		/// <param name="name">message</param>
		public FormatterNotFoundException(string name)
			: base($"formatter {name} Not Found")
		{ }

	}
}
