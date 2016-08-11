using CoreConfig = Microsoft.Extensions.Configuration;

namespace RpcLite.Config
{
	public class CoreConfigurationSection : CoreConfiguration, IConfigurationSection
	{
		public CoreConfigurationSection(CoreConfig.IConfiguration config)
			: base(config)
		{ 
			//Node = node;
		}

		public string Key { get { return ""; } }
		public string Path { get; }
		public string Value { get; set; }
	}
}
