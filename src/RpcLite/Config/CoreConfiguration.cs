#if NETCORE

using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml;
using CoreConfig = Microsoft.Extensions.Configuration;

namespace RpcLite.Config
{
	/// <summary>
	/// 
	/// </summary>
	public class CoreConfiguration : IRpcConfiguration
	{
		/// <summary>
		/// 
		/// </summary>
		protected CoreConfig.IConfiguration Node;

		/// <summary>
		/// 
		/// </summary>
		/// <param name="node"></param>
		public CoreConfiguration(CoreConfig.IConfiguration node)
		{
			Node = node;
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="key"></param>
		/// <returns></returns>
		public string this[string key]
		{
			get { return Node[key]; }
			set { Node[key] = value; }
		}

		/// <summary>
		/// 
		/// </summary>
		/// <returns></returns>
		public IEnumerable<IRpcConfigurationSection> GetChildren()
		{
			return Node.GetChildren()
				.Select(it => new CoreConfigurationSection(it));
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
			var node = Node.GetSection(key);
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

#endif
