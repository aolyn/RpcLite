using System;

namespace RpcLite.Config
{
	/// <summary>
	/// InvokerConfigItem
	/// </summary>
	public class InvokerConfig
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
		public InvokerConfig() { }

		//public IDictionary<string,string> Items { get; set; }

		/// <summary>
		/// 
		/// </summary>
		/// <param name="name"></param>
		/// <param name="type"></param>
		public InvokerConfig(string name, Type type)
		{
			Name = name;
			Type = RpcConfigHelper.GetTypeIdentifier(type);
		}

	}
}
