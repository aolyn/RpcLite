using System;
using System.Configuration;
using System.Linq;
using System.Xml;

namespace RpcLite.Config
{
	public class RpcLiteConfigSection : IConfigurationSectionHandler
	{
		/// <summary>
		/// Instance of RpcLiteConfigSection
		/// </summary>
		public static RpcLiteConfig Instance { get; private set; }

		public static void Initialize()
		{

		}

		static RpcLiteConfigSection()
		{
			try
			{
				var sec = ConfigurationManager.GetSection("RpcLite");
				Instance = sec as RpcLiteConfig;
				RpcLiteConfig.SetInstance(Instance);
			}
			catch (RpcLiteConfigurationErrorException)
			{
				throw;
			}
			catch (Exception ex)
			{
				throw new ConfigException("initialize config error", ex);
			}
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="parent"></param>
		/// <param name="configContext"></param>
		/// <param name="section"></param>
		/// <returns></returns>
		public object Create(object parent, object configContext, XmlNode section)
		{
			if (Instance != null)
				return Instance;

			Instance = RpcLiteConfigurationHelper.GetConfig(new XmlConfigurationSection(section));

			return Instance;
		}
	}

}