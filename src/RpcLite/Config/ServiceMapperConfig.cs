using System;

namespace RpcLite.Config
{
	/// <summary>
	/// ServiceMapperConfigItem
	/// </summary>
	public class ServiceMapperConfig
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
		public ServiceMapperConfig() { }

		/// <summary>
		/// 
		/// </summary>
		/// <param name="name"></param>
		/// <param name="type"></param>
		public ServiceMapperConfig(string name, Type type)
		{
			Name = name;
			Type = RpcConfigHelper.GetTypeIdentifier(type);
		}

	}
}
