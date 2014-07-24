using System;

namespace RpcLite
{
	public class ProcessRequestException : ServiceException
	{
		public ProcessRequestException() { }

		public ProcessRequestException(string message, Exception innerException)
			: base(message, innerException)
		{ }

		public ProcessRequestException(string message)
			: base(message)
		{ }
	}
}
