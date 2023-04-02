namespace RpcLite.Config
{
	/// <summary>
	/// Meta Info configuration
	/// </summary>
	public class MetaConfig
	{
		/// <summary>
		/// enable meta info api
		/// </summary>
		public bool Enabled { get; set; }

		/// <summary>
		/// meta info path, default /rpcliteinfo
		/// </summary>
		public string Path { get; set; }
	}
}
