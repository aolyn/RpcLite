using System;

namespace RpcLite.Client
{
	public class ClientException : ServiceException
	{
		public ClientException() { }

		public ClientException(string message, Exception innerException)
			: base(message, innerException)
		{ }

		public ClientException(string message)
			: base(message)
		{ }
	}
}
