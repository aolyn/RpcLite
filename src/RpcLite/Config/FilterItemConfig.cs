using System;

namespace RpcLite.Config
{
	/// <summary>
	/// FilterrConfigItem
	/// </summary>
	public class FilterItemConfig
	{
		/// <summary>
		/// name of Registry
		/// </summary>
		public string Name { get; set; }

		/// <summary>
		/// original configured type name, eg: ServiceImpl.ProductAsyncService,ServiceImpl
		/// </summary>
		public string Type { get; set; }

		/// <summary>
		/// 
		/// </summary>
		public FilterItemConfig() { }

		/// <summary>
		/// 
		/// </summary>
		/// <param name="name"></param>
		/// <param name="type"></param>
		public FilterItemConfig(string name, Type type)
		{
			Name = name;
			Type = RpcConfigHelper.GetTypeIdentifier(type);
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="type"></param>
		public FilterItemConfig(Type type)
			: this(null, type)
		{
		}

	}
}
