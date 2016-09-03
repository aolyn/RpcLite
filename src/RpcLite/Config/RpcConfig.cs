using System;
using System.Collections.Generic;

namespace RpcLite.Config
{
	/// <summary>
	/// RpcLite Config Section
	/// </summary>
	public class RpcConfig
	{

		#region Properties

		///// <summary>
		///// Service config items
		///// </summary>
		//[Obsolete("use Service.Services instead")]
		//public List<ServiceConfigItem> Services { get; set; } = new List<ServiceConfigItem>();

		/// <summary>
		/// Current app environment
		/// </summary>
		public string Environment { get; set; }

		///// <summary>
		///// Default environment of ServiceClient used to get ServiceClientAddress
		///// </summary>
		//public string ClientEnvironment { get; set; }

		///// <summary>
		///// Client config items
		///// </summary>
		//[Obsolete("use Client.Clients instead")]
		//public List<ClientConfigItem> Clients { get; set; } = new List<ClientConfigItem>();

		/// <summary>
		/// Client Address Resover
		/// </summary>
		[Obsolete]
		public ResolverConfigItem Resover { get; set; }

		/// <summary>
		/// Registry Configuration
		/// </summary>
		public RegistryConfig Registry { get; set; }

		/// <summary>
		/// App Id of current app
		/// </summary>
		public string AppId { get; set; }

		/// <summary>
		/// config file version
		/// </summary>
		public Version Version { get; internal set; }

		/// <summary>
		/// <para>service paths, used in module, if not null request path will be check</para>
		/// <para>or else if request path is not in paths process will not execute</para>
		/// </summary>
		public string[] ServicePaths { get; set; }

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

		#endregion

	}
}