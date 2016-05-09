using System.IO;

namespace RpcLite.Service
{
	/// <summary>
	/// 
	/// </summary>
	public class ServiceResponse
	{
		/// <summary>
		/// 
		/// </summary>
		public Stream ResponseStream { get; set; }

		/// <summary>
		/// 
		/// </summary>
		public string ContentType { get; set; }
	}
}
