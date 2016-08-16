using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RpcLite.Config
{
	public class ConfigManager
	{
		public static IConfiguration Default { get; private set; }

		public static void SetDefaultConfiguration(IConfiguration config)
		{
			Default = config;
		}

	}
}
