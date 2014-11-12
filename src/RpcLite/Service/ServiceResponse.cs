using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

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
		public Formatters.IFormatter Formatter { get; set; }
	}
}
