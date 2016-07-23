namespace RpcLite.Config
{
	/// <summary>
	/// ServiceConfigItem
	/// </summary>
	public class ServiceConfigItem
	{
		/// <summary>
		/// name of service
		/// </summary>
		public string Name { get; set; }

		/// <summary>
		/// relative path of service url, eg: ~/api/product
		/// </summary>
		public string Path { get; set; }

		/// <summary>
		/// relative path of service url, eg: ~/api/product
		/// </summary>
		public string Address { get; set; }

		/// <summary>
		/// Environment
		/// </summary>
		public string Environment { get; set; }

		/// <summary>
		/// assembly of service implement class
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
		/// get description string
		/// </summary>
		/// <returns></returns>
		public override string ToString()
		{
			return $"{Name}, { TypeName }, {Path}";
		}
	}
}
