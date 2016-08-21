using System.Configuration;
using RpcLite.Config;
using RpcLite.Service;

namespace RpcLite.AspNet
{
	/// <summary>
	/// 
	/// </summary>
	public class RpcLiteInitializer
	{
		/// <summary>
		/// 
		/// </summary>
		public static void Initialize()
		{
			var config = ConfigurationManager.GetSection("RpcLite") as RpcLiteConfig;
			RpcProcessor.Initialize(config);
		}

	}
}
