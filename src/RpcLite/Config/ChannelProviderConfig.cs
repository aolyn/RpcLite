using System;

namespace RpcLite.Config
{
	/// <summary>
	/// ChannelProviderItemConfig
	/// </summary>
	public class ChannelProviderConfig
	{
		/// <summary>
		/// name of ServiceMapper
		/// </summary>
		public string Name { get; set; }

		/// <summary>
		/// original configured type name, eg: ServiceImpl.ProductAsyncService,ServiceImpl
		/// </summary>
		public string Type { get; set; }

		/// <summary>
		/// 
		/// </summary>
		public ChannelProviderConfig() { }

		/// <summary>
		/// 
		/// </summary>
		/// <param name="name"></param>
		/// <param name="type"></param>
		public ChannelProviderConfig(string name, Type type)
		{
			Name = name;
			Type = RpcConfigHelper.GetTypeIdentifier(type);
		}

	}
}
