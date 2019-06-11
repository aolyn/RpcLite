using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml;

namespace RpcLite.Config
{
	/// <summary>
	/// 
	/// </summary>
	public class XmlConfiguration : IRpcConfiguration
	{
		private readonly XmlNode _node;

		/// <summary>
		/// 
		/// </summary>
		/// <param name="node"></param>
		public XmlConfiguration(XmlNode node)
		{
			_node = node;
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="key"></param>
		/// <returns></returns>
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

		/// <summary>
		/// 
		/// </summary>
		/// <returns></returns>
		public IEnumerable<IRpcConfigurationSection> GetChildren()
		{
			return _node.ChildNodes
				.Cast<XmlNode>()
				.Where(it => it.NodeType != XmlNodeType.Comment)
				.Select(it => new XmlConfigurationSection(it));
		}

		/// <summary>
		/// 
		/// </summary>
		public IEnumerable<IRpcConfigurationSection> Children => GetChildren();

		/// <summary>
		/// 
		/// </summary>
		/// <param name="key"></param>
		/// <returns></returns>
		public IRpcConfigurationSection GetSection(string key)
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
