using System;
using System.IO;

namespace RpcLite
{
	public class ServiceRequest
	{
		public Stream InputStream { get; set; }

		public string ActionName { get; set; }

		public Type ServiceType { get; set; }

		public ActionInfo ActionInfo { get; set; }

		public Formatters.IFormatter Formatter { get; set; }

		public object RequestObject { get; set; }
	}
}
