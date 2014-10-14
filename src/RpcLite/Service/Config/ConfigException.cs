using System;

namespace RpcLite.Config
{
	public class ConfigException : ServiceException
	{
		public ConfigException() { }

		public ConfigException(string message, Exception innerException)
			: base(message, innerException)
		{ }

		public ConfigException(string message)
			: base(message)
		{ }
	}
}
