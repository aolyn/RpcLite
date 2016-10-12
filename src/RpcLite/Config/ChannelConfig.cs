using System.Collections.Generic;

namespace RpcLite.Config
{
	/// <summary>
	/// 
	/// </summary>
	public class ChannelConfig
	{
		/// <summary>
		/// 
		/// </summary>
		public List<ChannelProviderConfig> Providers { get; set; } = new List<ChannelProviderConfig>();

		///// <summary>
		///// 
		///// </summary>
		//public bool RemoveDefault { get; set; }
	}
}
