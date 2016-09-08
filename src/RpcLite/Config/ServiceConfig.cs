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
	}
}
