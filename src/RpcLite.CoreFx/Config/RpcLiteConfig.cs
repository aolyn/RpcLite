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

		private static RpcLiteConfig _instance;
		/// <summary>
		/// Instance of RpcLiteConfigSection
		/// </summary>
		public static RpcLiteConfig Instance
		{
			get
			{
				if (_instance != null)
					return _instance;

				_instance = new RpcLiteConfig();

				return _instance;
			}
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="instance"></param>
		/// <returns></returns>
		public static void SetInstance(RpcLiteConfig instance)
		{
			_instance = instance;
		}

		private readonly List<ServiceConfigItem> _services = new List<ServiceConfigItem>();
		/// <summary>
		/// Service config items
		/// </summary>
		public List<ServiceConfigItem> Services { get { return _services; } }

		/// <summary>
		/// Current app environment
		/// </summary>
		public string Environment { get; set; }

		/// <summary>
		/// Default environment of ServiceClient used to get ServiceClientAddress
		/// </summary>
		public string ClientEnvironment { get; set; }

		private readonly List<ClientConfigItem> _clients = new List<ClientConfigItem>();
		/// <summary>
		/// Client config items
		/// </summary>
		public List<ClientConfigItem> Clients
		{
			get { return _clients; }
		}

		/// <summary>
		/// Client Address Resover
		/// </summary>
		public ResolverConfigItem Resover { get; set; }

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

	}
}