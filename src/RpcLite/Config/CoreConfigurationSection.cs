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
		/// <param name="config"></param>
		public CoreConfigurationSection(CoreConfig.IConfigurationSection config)
			: base(config)
		{
			//Node = node;
		}

		/// <summary>
		/// 
		/// </summary>
		public string Key => (Node as CoreConfig.IConfigurationSection)?.Key;

		/// <summary>
		/// 
		/// </summary>
		public string Path => (Node as CoreConfig.IConfigurationSection)?.Path;

		/// <summary>
		/// 
		/// </summary>
		public string Value
		{
			get => (Node as CoreConfig.IConfigurationSection)?.Value;
			set
			{
				if (Node is CoreConfig.IConfigurationSection node)
					node.Value = value;
			}
		}

	}
}

#endif