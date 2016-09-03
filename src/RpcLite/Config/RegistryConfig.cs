using System;

namespace RpcLite.Config
{
	/// <summary>
	/// RegistryConfigItem
	/// </summary>
	public class RegistryConfig
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
		public RegistryConfig() { }

		/// <summary>
		/// 
		/// </summary>
		/// <param name="name"></param>
		/// <param name="type"></param>
		/// <param name="address"></param>
		public RegistryConfig(string name, Type type, string address)
		{
			Name = name;
			Type = RpcConfigHelper.GetTypeIdentifier(type);
			Address = address;
		}

		///// <summary>
		///// Assembly Name of Registry
		///// </summary>
		//public string AssemblyName { get; set; }

		///// <summary>
		///// full service type name, eg: ServiceImpl.ProductAsyncService
		///// </summary>
		//public string TypeName { get; set; }
	}
}
