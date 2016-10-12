using System.IO;

namespace RpcLite.Service
{
	/// <summary>
	/// 
	/// </summary>
	public class ServiceRequest
	{
		/// <summary>
		/// 
		/// </summary>
		public Stream RequestStream { get; set; }

		/// <summary>
		/// 
		/// </summary>
		public string ActionName { get; set; }

		///// <summary>
		///// 
		///// </summary>
		//public Type ServiceType { get; set; }

		///// <summary>
		///// 
		///// </summary>
		//public RpcAction ActionInfo { get; set; }

		/// <summary>
		/// 
		/// </summary>
		public string Path { get; set; }

		/// <summary>
		/// 
		/// </summary>
		public string ContentType { get; set; }

		/// <summary>
		/// 
		/// </summary>
		public int ContentLength { get; set; }

		/// <summary>
		/// 
		/// </summary>
		public RequestType RequestType { get; set; }
	}
}
