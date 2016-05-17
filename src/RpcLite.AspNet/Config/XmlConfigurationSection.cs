using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml;

namespace RpcLite.Config
{
	public class XmlConfigurationSection : IConfigurationSection
	{
		private XmlNode _node;

		public XmlConfigurationSection(XmlNode node)
		{
			_node = node;
		}

		public IEnumerable<IConfigurationSection> GetChildren()
		{
			return _node.ChildNodes
				.Cast<XmlNode>()
				.Select(it => new XmlConfigurationSection(it));
		}

		public string this[string key]
		{
			get { return _node.Name; }
			set { throw new NotImplementedException(); }
		}

		public IConfigurationSection GetSection(string key)
		{
			var node = _node[key];
			return node == null
				? null
				: new XmlConfigurationSection(node);
		}

		public string Key { get { return _node.Name; } }
		public string Path { get; }
		public string Value { get; set; }
	}
}
