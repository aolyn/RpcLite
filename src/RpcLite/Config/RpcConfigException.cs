using System;

namespace RpcLite.Config
{
	/// <summary>
	/// 
	/// </summary>
	public class RpcConfigException : Exception
	{
		/// <summary>
		/// 
		/// </summary>
		/// <param name="message"></param>
		public RpcConfigException(string message) :
			base(message)
		{

		}
	}
}
