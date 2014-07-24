using System;

namespace RpcLite
{
	public class ServiceException : Exception
	{
		public ServiceException() { }

		public ServiceException(string message, Exception innerException)
			: base(message, innerException)
		{ }

		public ServiceException(string message)
			: base(message)
		{ }
	}

}
