using System;

#if NETCORE
using CoreConfig = Microsoft.Extensions.Configuration;
#endif

namespace RpcLite.Config
{
	/// <summary>
	/// used to get RpcLiteConfig from IConfiguration
	/// </summary>
	public class RpcConfigHelper
	{
#if NETCORE
		/// <summary>
		/// get RpcLiteConfig from IConfiguration
		/// </summary>
		/// <param name="config"></param>
		/// <returns></returns>
		public static RpcConfig GetConfig(CoreConfig.IConfiguration config)
		{
			return GetConfig(new CoreConfigurationSection(config));
		}
#endif

		/// <summary>
		/// get RpcLiteConfig from IConfiguration
		/// </summary>
		/// <param name="config"></param>
		/// <returns></returns>
		public static RpcConfig GetConfig(IRpcConfiguration config)
		{
			var version = GetVersion(config);

			IRpcConfigParser parser;
			if (version <= new Version(1, 0))
			{
				parser = new RpcConfigParserV1();
			}
			//else if (version == new Version(1, 1))
			//{
			//	parser = new RpcLiteParserV11();
			//}
			else
			{
				parser = new RpcConfigParserV11();
			}

			var instance = parser.GetConfig(config);
			return instance;
		}

		internal static Version GetVersion(IRpcConfiguration config)
		{
			var versionText = config["version"];
			Version version;
			if (versionText == null || !Version.TryParse(versionText, out version))
				version = new Version("1.0");
			return version;
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="type"></param>
		/// <returns></returns>
		public static string GetTypeIdentifier(Type type)
		{
			return type.FullName + ", " + type.Assembly.GetName().Name;
		}
	}

	internal interface IRpcConfigParser
	{
		RpcConfig GetConfig(IRpcConfiguration config);
	}

}
