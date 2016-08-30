using System.Xml;

namespace RpcLite.Config
{
	/// <summary>
	/// 
	/// </summary>
	public class XmlConfigurationSection : XmlConfiguration, IConfigurationSection
	{
		/// <summary>
		/// 
		/// </summary>
		private XmlNode _node;

		/// <summary>
		/// 
		/// </summary>
		/// <param name="node"></param>
		public XmlConfigurationSection(XmlNode node)
			: base(node)
		{
			_node = node;
		}

		/// <summary>
		/// 
		/// </summary>
		public string Key { get { return _node.Name; } }

		/// <summary>
		/// 
		/// </summary>
		public string Path { get; }

		/// <summary>
		/// 
		/// </summary>
		public string Value { get; set; }
	}
}
