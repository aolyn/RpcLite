
namespace ServiceRegistry.Contract
{
	public class GetServiceAddressRequest
	{
		public string ServiceName { get; set; }
		public string Namespace { get; set; }
		public string Environment { get; set; }
	}

}