namespace RpcLite.Config
{
	/// <summary>
	/// RegistryConfigItem
	/// </summary>
	public class RegistryConfigItem
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
		/// Assembly Name of Registry
		/// </summary>
		public string AssemblyName { get; set; }

		/// <summary>
		/// full service type name, eg: ServiceImpl.ProductAsyncService
		/// </summary>
		public string TypeName { get; set; }
	}
}
