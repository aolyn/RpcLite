using System;
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
		public Stream InputStream { get; set; }

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
		public ActionInfo ActionInfo { get; set; }

		/// <summary>
		/// 
		/// </summary>
		public Formatters.IFormatter Formatter { get; set; }

		/// <summary>
		/// 
		/// </summary>
		public object RequestObject { get; set; }
	}
}
