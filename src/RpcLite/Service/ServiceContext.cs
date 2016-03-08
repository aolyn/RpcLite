using System;
using System.Web;

namespace RpcLite.Service
{

	/// <summary>
	/// 
	/// </summary>
	public class ServiceContext
	{

		/// <summary>
		/// 
		/// </summary>
		internal object ServiceContainer { get; set; }

		/// <summary>
		/// 
		/// </summary>
		internal object State { get; set; }

		/// <summary>
		/// 
		/// </summary>
		internal RpcAction Action { get; set; }

		/// <summary>
		/// 
		/// </summary>
		public ServiceResponse Response { get; set; }

		/// <summary>
		/// 
		/// </summary>
		public object Result { get; set; }

		/// <summary>
		/// 
		/// </summary>
		public object Tag { get; set; }

		/// <summary>
		/// 
		/// </summary>
		public ServiceRequest Request { get; set; }

		/// <summary>
		/// 
		/// </summary>
		public object Argument { get; set; }

		/// <summary>
		/// 
		/// </summary>
		public RpcService Service { get; set; }

		/// <summary>
		/// 
		/// </summary>
		public Formatters.IFormatter Formatter { get; set; }

		/// <summary>
		/// 
		/// </summary>
		public object ExtraData { get; internal set; }

		/// <summary>
		/// 
		/// </summary>
		public HttpContext HttpContext { get; internal set; }

		/// <summary>
		/// 
		/// </summary>
		public Exception Exception { get; internal set; }
	}
}
