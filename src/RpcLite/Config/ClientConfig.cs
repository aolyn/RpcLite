using System.Collections.Generic;

namespace RpcLite.Config
{
	/// <summary>
	/// 
	/// </summary>
	public class ClientConfig
	{
		/// <summary>
		/// 
		/// </summary>
		public List<ClientConfigItem> Clients { get; set; } = new List<ClientConfigItem>();

		/// <summary>
		/// 
		/// </summary>
		public InvokerConfig Invoker { get; set; }
	}
}
