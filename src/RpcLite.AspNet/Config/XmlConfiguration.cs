using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml;

namespace RpcLite.Config
{
	public class XmlConfiguration : IConfiguration
	{
		private XmlNode _node;

		public XmlConfiguration(XmlNode node)
		{
			_node = node;
		}

		public string this[string key]
		{
			get
			{
				var attrValue = RpcLiteConfigurationHelper.GetAttribute(key, _node);
				if (attrValue != null)
					return attrValue;

				var node = _node[key];
				return node?.InnerText;
			}

			set
			{
				throw new NotImplementedException();
			}
		}

		public IEnumerable<IConfigurationSection> GetChildren()
		{
			return _node.ChildNodes
				.Cast<XmlNode>()
				.Select(it => new XmlConfigurationSection(it));
		}

		public IConfigurationSection GetSection(string key)
		{
			var node = _node[key];
			return node == null
				? null
				: new XmlConfigurationSection(node);
		}
	}
}
