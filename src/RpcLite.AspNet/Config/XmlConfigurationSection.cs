using System.Collections.Generic;
using System.Xml;

namespace RpcLite.Config
{
	public class XmlConfigurationSection : XmlConfiguration, IConfigurationSection
	{
		private XmlNode _node;

		public XmlConfigurationSection(XmlNode node)
			: base(node)
		{
			_node = node;
		}

		public string Key { get { return _node.Name; } }
		public string Path { get; }
		public string Value { get; set; }
		public IEnumerable<IConfigurationSection> Children => GetChildren();
	}
}
