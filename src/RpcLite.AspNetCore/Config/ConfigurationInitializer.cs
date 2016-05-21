using CoreConfig = Microsoft.Extensions.Configuration;

namespace RpcLite.Config
{
	public class ConfigurationInitializer
	{
		public static void Initialize(CoreConfig.IConfiguration config)
		{
			//var config = new ConfigurationBuilder()
			//	.AddJsonFile(jsonFile)
			//	.Build();

			//var config = new ConfigurationBuilder()
			//	.AddXmlFile("rpclite.config.xml")
			//	.Build();

			var rpcConfig = RpcLiteConfigurationHelper.GetConfig(new CoreConfigurationSection(config));
			RpcLiteConfig.SetInstance(rpcConfig);
		}
	}
}
