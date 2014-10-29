using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RpcLite.Service
{
	public class ServiceResponse
	{
		public System.IO.Stream ResponseStream { get; set; }

		public Formatters.IFormatter Formatter { get; set; }
	}
}
