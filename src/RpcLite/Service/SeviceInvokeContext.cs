using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RpcLite.Service
{

	/// <summary>
	/// 
	/// </summary>
	public class SeviceInvokeContext
	{

		/// <summary>
		/// 
		/// </summary>
		public object Service { get; set; }

		/// <summary>
		/// 
		/// </summary>
		public object State { get; set; }

		/// <summary>
		/// 
		/// </summary>
		public ActionInfo Action { get; set; }

		/// <summary>
		/// 
		/// </summary>
		public ServiceResponse Response { get; set; }

		/// <summary>
		/// 
		/// </summary>
		public object SyncResult { get; set; }

		/// <summary>
		/// 
		/// </summary>
		public object Tag { get; set; }
	}
}
