﻿
namespace ServiceRegistery.Contract
{
	public class GetServiceAddressRequest
	{
		public string ServiceName { get; set; }
		public string Namespace { get; set; }
		public string Environment { get; set; }
	}

	public class GetServiceAddressResponse
	{
		public string ServiceName { get; set; }
		public string Namespace { get; set; }
		public string Environment { get; set; }
		public string Address { get; set; }
	}
}