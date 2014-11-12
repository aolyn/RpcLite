using System;

namespace RpcLite
{
	/// <summary>
	/// Respresnts service infomation
	/// </summary>
	public class ServiceInfo
	{
		/// <summary>
		/// Service request url path
		/// </summary>
		public string Path { get; set; }

		/// <summary>
		/// Service's Type
		/// </summary>
		public Type Type { get; set; }

		/// <summary>
		/// Name of Service
		/// </summary>
		public string Name { get; set; }

		/// <summary>
		/// Convert to string
		/// </summary>
		/// <returns></returns>
		public override string ToString()
		{
			return string.Format("{0}, {1}, {2}", Name, Path, Type);
		}
	}
}
