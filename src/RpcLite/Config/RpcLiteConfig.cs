using System;
using System.Collections.Generic;

namespace RpcLite.Config
{
	/// <summary>
	/// RpcLite Config Section
	/// </summary>
	public class RpcLiteConfig
	{
		//private string _clientEnvironmentAttributeValue;

		//private static RpcLiteConfig _instance;
		///// <summary>
		///// Instance of RpcLiteConfigSection
		///// </summary>
		//private static RpcLiteConfig Instance
		//{
		//	get
		//	{
		//		if (_instance != null)
		//			return _instance;

		//		_instance = new RpcLiteConfig();

		//		return _instance;
		//	}
		//}

		///// <summary>
		///// 
		///// </summary>
		///// <param name="instance"></param>
		///// <returns></returns>
		//public static void SetInstance(RpcLiteConfig instance)
		//{
		//	_instance = instance;
		//}

		#region Properties

		/// <summary>
		/// Service config items
		/// </summary>
		public List<ServiceConfigItem> Services { get; set; } = new List<ServiceConfigItem>();

		/// <summary>
		/// Current app environment
		/// </summary>
		public string Environment { get; set; }

		///// <summary>
		///// Default environment of ServiceClient used to get ServiceClientAddress
		///// </summary>
		//public string ClientEnvironment { get; set; }

		/// <summary>
		/// Client config items
		/// </summary>
		public List<ClientConfigItem> Clients { get; set; } = new List<ClientConfigItem>();

		/// <summary>
		/// Client Address Resover
		/// </summary>
		public ResolverConfigItem Resover { get; set; }

		/// <summary>
		/// Registry Configuration
		/// </summary>
		public RegistryConfigItem Registry { get; set; }

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
		public MonitorConfigItem Monitor { get; set; }

		#endregion

	}
}