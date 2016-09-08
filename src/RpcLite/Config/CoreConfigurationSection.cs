#if NETCORE

using CoreConfig = Microsoft.Extensions.Configuration;

namespace RpcLite.Config
{
	/// <summary>
	/// 
	/// </summary>
	public class CoreConfigurationSection : CoreConfiguration, IConfigurationSection
	{
		/// <summary>
		/// 
		/// </summary>
		/// <param name="config"></param>
		public CoreConfigurationSection(CoreConfig.IConfiguration config)
			: base(config)
		{ 
			//Node = node;
		}

		/// <summary>
		/// 
		/// </summary>
		public string Key { get { return ""; } }

		/// <summary>
		/// 
		/// </summary>
		public string Path { get; }

		/// <summary>
		/// 
		/// </summary>
		public string Value { get; set; }
	}
}

#endif