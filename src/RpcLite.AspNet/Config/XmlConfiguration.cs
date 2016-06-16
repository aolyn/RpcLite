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
				var attrValue = GetAttribute(key, _node);
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
				.Where(it => it.NodeType != XmlNodeType.Comment)
				.Select(it => new XmlConfigurationSection(it));
		}

		public IEnumerable<IConfigurationSection> Children => GetChildren();

		public IConfigurationSection GetSection(string key)
		{
			var node = _node[key];
			return node == null
				? null
				: new XmlConfigurationSection(node);
		}

		/// <summary>
		/// get attribute value if attribute not exist returns null
		/// </summary>
		/// <param name="attributeName"></param>
		/// <param name="node"></param>
		/// <returns></returns>
		public static string GetAttribute(string attributeName, XmlNode node)
		{
			if (attributeName == null)
				throw new ArgumentNullException("attributeName");
			if (node == null)
				throw new ArgumentNullException("node");

			return node.Attributes?[attributeName]?.Value;
		}

	}
}
