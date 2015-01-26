using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

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
		internal ActionInfo Action { get; set; }

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
		public ServiceInfo Service { get; set; }
	}
}
