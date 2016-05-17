using System;

namespace RpcLite.Config
{
	/// <summary>
	/// 
	/// </summary>
	public class RpcLiteConfigurationErrorException : Exception
	{
		/// <summary>
		/// 
		/// </summary>
		/// <param name="message"></param>
		public RpcLiteConfigurationErrorException(string message) :
			base(message)
		{

		}
	}
}
