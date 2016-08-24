using System;

namespace RpcLite.Config
{
	/// <summary>
	/// MonitorConfigItem
	/// </summary>
	public class MonitorConfigItem
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
		/// address of Registry
		/// </summary>
		public string Address { get; set; }

		/// <summary>
		/// 
		/// </summary>
		public MonitorConfigItem() { }

		/// <summary>
		/// 
		/// </summary>
		/// <param name="name"></param>
		/// <param name="type"></param>
		public MonitorConfigItem(string name, Type type)
			: this(name, type, null)
		{
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="name"></param>
		/// <param name="type"></param>
		/// <param name="address"></param>
		public MonitorConfigItem(string name, Type type, string address)
		{
			Name = name;
			Type = RpcConfigHelper.GetTypeIdentifier(type);
			Address = address;
		}

	}
}
