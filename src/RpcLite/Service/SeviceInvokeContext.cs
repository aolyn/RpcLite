using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RpcLite.Service
{
	public class SeviceInvokeContext
	{
		public object Service { get; set; }
		public object State { get; set; }


		public ActionInfo Action { get; set; }

		public ServiceResponse Output { get; set; }
	}
}
