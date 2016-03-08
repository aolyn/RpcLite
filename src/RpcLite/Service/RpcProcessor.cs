using System.Linq;
using RpcLite.Config;
using RpcLite.Formatters;

namespace RpcLite.Service
{
	/// <summary>
	/// 
	/// </summary>
	internal static class RpcProcessor
	{
		/// <summary>
		/// 
		/// </summary>
		/// <param name="contentType"></param>
		/// <returns></returns>
		public static IFormatter GetFormatter(string contentType)
		{
			IFormatter formatter;
			if (!string.IsNullOrEmpty(contentType))
			{
				formatter = GlobalConfig.Formaters.FirstOrDefault(it => it.SupportMimes.Contains(contentType));
				if (formatter == null)
					throw new ConfigException("Not Supported MIME: " + contentType);
			}
			else
			{
				if (GlobalConfig.Formaters.Count == 0)
					throw new ConfigException("Configuration error: no formatters.");

				formatter = GlobalConfig.Formaters[0];
				if (formatter.SupportMimes.Count == 0)
					throw new ConfigException("Configuration error: formatter " + formatter.GetType() + " has no support MIME");
			}
			return formatter;
		}
	}
}