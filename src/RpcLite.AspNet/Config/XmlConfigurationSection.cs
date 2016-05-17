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

		//public IEnumerable<IConfigurationSection> GetChildren()
		//{
		//	return _node.ChildNodes
		//		.Cast<XmlNode>()
		//		.Where(it => it.NodeType != XmlNodeType.Comment)
		//		.Select(it => new XmlConfigurationSection(it));
		//}

		//public string this[string key]
		//{
		//	get
		//	{
		//		var attrValue = XmlConfiguration.GetAttribute(key, _node);
		//		if (attrValue != null)
		//			return attrValue;

		//		var node = _node[key];
		//		return node?.InnerText;
		//	}
		//	set { throw new NotImplementedException(); }
		//}

		//public IConfigurationSection GetSection(string key)
		//{
		//	var node = _node[key];
		//	return node == null
		//		? null
		//		: new XmlConfigurationSection(node);
		//}

		public string Key { get { return _node.Name; } }
		public string Path { get; }
		public string Value { get; set; }
	}
}
