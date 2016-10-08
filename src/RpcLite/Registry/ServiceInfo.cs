namespace RpcLite.Registry
{
	/// <summary>
	/// 
	/// </summary>
	public class ServiceInfo
	{
		/// <summary>
		/// 
		/// </summary>
		public string Name { get; set; }

		/// <summary>
		/// 
		/// </summary>
		public string Group { get; set; }

		/// <summary>
		/// 
		/// </summary>
		public string Address { get; set; }

		/// <summary>
		/// extra data such as Support Formatters
		/// </summary>
		public string Data { get; set; }

		/// <summary>
		/// 
		/// </summary>
		/// <returns></returns>
		public override string ToString()
		{
			return $"{Name}, {Group}, {Address}";
		}

	}
}
