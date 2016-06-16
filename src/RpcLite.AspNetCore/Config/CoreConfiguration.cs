using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml;
using CoreConfig = Microsoft.Extensions.Configuration;

namespace RpcLite.Config
{
	public class CoreConfiguration : IConfiguration
	{
		protected CoreConfig.IConfiguration _node;

		public CoreConfiguration(CoreConfig.IConfiguration node)
		{
			_node = node;
		}

		public string this[string key]
		{
			get { return _node[key]; }
			set { _node[key] = value; }
		}

		public IEnumerable<IConfigurationSection> GetChildren()
		{
			return _node.GetChildren()
				.Select(it => new CoreConfigurationSection(it));
		}

		public IEnumerable<IConfigurationSection> Children => GetChildren();

		public IConfigurationSection GetSection(string key)
		{
			var node = _node.GetSection(key);
			return node == null
				? null
				: new CoreConfigurationSection(node);
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
