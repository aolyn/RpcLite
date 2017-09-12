using System.Collections.Generic;

namespace RpcLite.Config
{
	/// <summary>
	/// 
	/// </summary>
	public class ServiceConfig
	{
		/// <summary>
		/// 
		/// </summary>
		public ServiceMapperConfig Mapper { get; set; }

		/// <summary>
		/// 
		/// </summary>
		public List<ServiceConfigItem> Services { get; set; } = new List<ServiceConfigItem>();

		///// <summary>
		///// <para>service paths, used in module, if not null request path will be check</para>
		///// <para>or else if request path is not in paths process will not execute</para>
		///// </summary>
		//public string[] Paths { get; set; }
	}
}
