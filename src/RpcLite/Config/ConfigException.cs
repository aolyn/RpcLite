using System;

namespace RpcLite.Config
{
	/// <summary>
	/// Represent error of configuration
	/// </summary>
	public class ConfigException : ServiceException
	{
		/// <summary>
		/// Initializes a new instance of RpcLite.Config.ConfigException class
		/// </summary>
		public ConfigException() { }

		/// <summary>
		/// Initializes a new instance of RpcLite.RequestException class with specifid message and inner exception
		/// </summary>
		/// <param name="message">message</param>
		/// <param name="innerException">inner exception</param>
		public ConfigException(string message, Exception innerException)
			: base(message, innerException)
		{ }

		/// <summary>
		/// Initializes a new instance of RpcLite.RequestException class with specifid message
		/// </summary>
		/// <param name="message">message</param>
		public ConfigException(string message)
			: base(message)
		{ }
	}
}
