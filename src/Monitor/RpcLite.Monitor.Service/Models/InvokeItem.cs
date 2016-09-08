using System;

namespace RpcLite.Monitor.Service.Models
{
	public class InvokeItem
	{
		public long Id { get; set; }

		public string Service { get; set; }

		public string Action { get; set; }

		public DateTime StartDate { get; set; }

		public DateTime EndDate { get; set; }

		public int Duration { get; set; }
	}
}
