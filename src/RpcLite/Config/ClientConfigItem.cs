namespace RpcLite.Config
{
	/// <summary>
	/// client configuration item
	/// </summary>
	public class ClientConfigItem
	{
		/// <summary>
		/// name of service
		/// </summary>
		public string Name { get; set; }

		/// <summary>
		/// namespace
		/// </summary>
		public string Namespace { get; set; }

		/// <summary>
		/// environment
		/// </summary>
		public string Environment { get; set; }

		/// <summary>
		/// service url, eg: http://localhost/api/product
		/// </summary>
		public string Path { get; set; }

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
		/// extra attribute of service
		/// </summary>
		public string[] Items { get; set; }

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
