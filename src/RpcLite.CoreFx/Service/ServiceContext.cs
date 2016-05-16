using System;

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
		public object ServiceContainer { get; set; }

		/// <summary>
		/// 
		/// </summary>
		public object State { get; set; }

		/// <summary>
		/// 
		/// </summary>
		public RpcAction Action { get; set; }

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
		public object ExtraData { get; set; }

		/// <summary>
		/// 
		/// </summary>
		public object ExecutingContext { get; set; }

		/// <summary>
		/// 
		/// </summary>
		public Exception Exception { get; set; }
	}
}
