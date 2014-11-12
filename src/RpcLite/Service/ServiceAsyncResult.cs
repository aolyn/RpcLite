using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RpcLite.Service
{
	/// <summary>
	/// 
	/// </summary>
	public class ServiceAsyncResult : IAsyncResult
	{
		/// <summary>
		/// 
		/// </summary>
		public object AsyncState { get; set; }

		/// <summary>
		/// 
		/// </summary>
		public System.Threading.WaitHandle AsyncWaitHandle { get; set; }

		/// <summary>
		/// 
		/// </summary>
		public bool CompletedSynchronously { get; set; }

		/// <summary>
		/// 
		/// </summary>
		public bool IsCompleted { get; set; }

		/// <summary>
		/// 
		/// </summary>
		public object HttpContext { get; set; }

		/// <summary>
		/// 
		/// </summary>
		public object SyncResult { get; set; }
	}
}
