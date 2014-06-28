using System;

namespace RpcLite
{
	public class RequestException : ServiceException
	{
		public RequestException() { }

		public RequestException(string message, Exception innerException)
			: base(message, innerException)
		{ }

		public RequestException(string message)
			: base(message)
		{ }
	}
}
