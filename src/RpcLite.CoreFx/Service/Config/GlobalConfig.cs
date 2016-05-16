using System.Collections.Generic;
using RpcLite.Formatters;

namespace RpcLite.Config
{
	/// <summary>
	/// 
	/// </summary>
	public static class GlobalConfig
	{
		/// <summary>
		/// 
		/// </summary>
		public static readonly List<IFormatter> Formaters = new List<IFormatter>();

		static GlobalConfig()
		{
			Formaters.Add(new JsonFormatter());
			//Formaters.Add(new XmlFormatter());
		}
	}
}
