using System;
using System.IO;
using System.Web;

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

		/// <summary>
		/// 
		/// </summary>
		public Type ServiceType { get; set; }

		/// <summary>
		/// 
		/// </summary>
		public RpcAction ActionInfo { get; set; }

	}
}
