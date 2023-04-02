using System;

namespace RpcLite.Config
{
	/// <summary>
	/// RpcLite Config Section
	/// </summary>
	public class RpcConfig
	{

		#region Properties

		/// <summary>
		/// Current app environment
		/// </summary>
		public string Group { get; set; }

		/// <summary>
		/// Registry Configuration
		/// </summary>
		public RegistryConfig Registry { get; set; }

		/// <summary>
		/// Meta info config, for example /rpcliteinfo
		/// </summary>
		public MetaConfig Meta { get; set; }

		/// <summary>
		/// App Id of current app
		/// </summary>
		public string AppId { get; set; }

		/// <summary>
		/// config file version
		/// </summary>
		public Version Version { get; internal set; }

		/// <summary>
		/// 
		/// </summary>
		public MonitorConfig Monitor { get; set; }

		/// <summary>
		/// 
		/// </summary>
		public ServiceConfig Service { get; set; }

		/// <summary>
		/// 
		/// </summary>
		public ClientConfig Client { get; set; }

		/// <summary>
		/// 
		/// </summary>
		public FilterConfig Filter { get; set; }

		/// <summary>
		/// 
		/// </summary>
		public FormatterConfig Formatter { get; set; }

		#endregion

	}
}