using System.Collections.Generic;

namespace RpcLite.Config
{
	/// <summary>
	/// 
	/// </summary>
	public class FormatterConfig
	{
		/// <summary>
		/// 
		/// </summary>
		public List<FormatterItemConfig> Formatters { get; set; } = new List<FormatterItemConfig>();

		/// <summary>
		/// 
		/// </summary>
		public bool RemoveDefault { get; set; }
	}
}
