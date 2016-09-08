using System;

namespace RpcLite.Monitor.Contract
{
	public class InvokeInfo
	{
		public string Id { get; set; }

		public string Service { get; set; }

		public string Action { get; set; }

		public DateTime StartDate { get; set; }

		public DateTime EndDate { get; set; }

		public int Duration { get; set; }
	}
}
