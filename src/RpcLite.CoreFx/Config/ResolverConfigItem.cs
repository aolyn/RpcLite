namespace RpcLite.Config
{
	/// <summary>
	/// ResolverConfigItem
	/// </summary>
	public class ResolverConfigItem
	{
		/// <summary>
		/// Assembly Name of Resolver
		/// </summary>
		public string AssemblyName { get; set; }

		/// <summary>
		/// full service type name, eg: ServiceImpl.ProductAsyncService
		/// </summary>
		public string TypeName { get; set; }

		/// <summary>
		/// original configured type name, eg: ServiceImpl.ProductAsyncService,ServiceImpl
		/// </summary>
		public string Type { get; set; }

		/// <summary>
		/// name of Resolver
		/// </summary>
		public string Name { get; set; }

		/// <summary>
		/// address of Resolver
		/// </summary>
		public string Address { get; set; }
	}

}
