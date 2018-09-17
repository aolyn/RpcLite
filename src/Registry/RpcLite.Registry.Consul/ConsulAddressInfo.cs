using Consul;

namespace RpcLite.Registry.Consul
{
	internal class ConsulAddressInfo
	{
		public ConsulClientConfiguration[] Servers { get; set; }

		public int Ttl { get; set; }

		/// <summary>
		/// -1 means not setted
		/// </summary>
		public int CheckInterval { get; set; }
	}
}