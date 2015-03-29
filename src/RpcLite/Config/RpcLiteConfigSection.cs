using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Xml;

namespace RpcLite.Config
{
	public class RpcLiteConfigSection : IConfigurationSectionHandler
	{
		private static RpcLiteConfigSection _instance;
		public static RpcLiteConfigSection Instance
		{
			get
			{
				if (_instance != null)
					return _instance;

				_instance = new RpcLiteConfigSection();

				return _instance;
			}
		}

		private readonly List<ServiceConfigItem> _services = new List<ServiceConfigItem>();
		public List<ServiceConfigItem> Services
		{
			get { return _services; }
		}

		private readonly List<ClientConfigItem> _clients = new List<ClientConfigItem>();
		public List<ClientConfigItem> Clients
		{
			get { return _clients; }
		}

		public ResolverConfigItem Resover { get; private set; }

		static RpcLiteConfigSection()
		{
			try
			{
				var sec = ConfigurationManager.GetSection("RpcLite");
				_instance = sec as RpcLiteConfigSection;
			}
			catch (Exception ex)
			{
				throw new ConfigException("initialize config error",ex);
			}
		}

		public object Create(object parent, object configContext, XmlNode section)
		{
			if (_instance != null)
				return _instance;

			_instance = new RpcLiteConfigSection();

			InitializeResolverConfig(section);
			InitializeServiceConfig(section);
			InitializeClientConfig(section);

			return this;
		}

		private void InitializeClientConfig(XmlNode section)
		{
			try
			{
				var servicesNode = section.SelectSingleNode("clients");
				if (servicesNode != null)
				{
					foreach (XmlNode item in servicesNode.ChildNodes)
					{
						if (item.Name != "add" || item.Attributes == null)
							continue;

						var name = item.Attributes.Cast<XmlAttribute>().Where(it => it.Name == "name").Select(it => it.Value).FirstOrDefault();
						var path = item.Attributes.Cast<XmlAttribute>().Where(it => it.Name == "path").Select(it => it.Value).FirstOrDefault();
						var type = item.Attributes.Cast<XmlAttribute>().Where(it => it.Name == "type").Select(it => it.Value).FirstOrDefault();

						if (string.IsNullOrEmpty(name))
							throw new ConfigurationErrorsException("name of  RpcLite configuration node can't be null or empty");
						if (string.IsNullOrEmpty(path))
							throw new ConfigurationErrorsException("path of  RpcLite configuration node can't be null or empty");
						if (string.IsNullOrEmpty(type))
							throw new ConfigurationErrorsException("type of  RpcLite configuration node can't be null or empty");

						string typeName;
						string assemblyName;

						var splitorIndex = type.IndexOf(",", StringComparison.Ordinal);
						if (splitorIndex > -1)
						{
							typeName = type.Substring(0, splitorIndex);
							assemblyName = type.Substring(splitorIndex + 1);
						}
						else
						{
							typeName = type;
							assemblyName = null;
						}

						var serviceConfigItem = new ClientConfigItem
						{
							Name = name,
							Type = type,
							TypeName = typeName,
							AssemblyName = assemblyName,
							Path = path,
						};
						Clients.Add(serviceConfigItem);
					}
				}
			}
			catch (Exception ex)
			{
				throw new ConfigException("Client Configuration Error", ex);
			}
		}

		private void InitializeResolverConfig(XmlNode section)
		{
			try
			{
				var resolverNode = section.SelectSingleNode("addressResolver");
				if (resolverNode != null)
				{
					//foreach (XmlNode item in resolverNode.ChildNodes)
					{
						//if (resolverNode.Name != "add" || resolverNode.Attributes == null)
						//	continue;

						var name = GetAttribute("name", resolverNode);
						var type = GetAttribute("type", resolverNode);

						if (string.IsNullOrEmpty(name))
							throw new ConfigurationErrorsException("name of  RpcLite configuration node can't be null or empty");
						if (string.IsNullOrEmpty(type))
							throw new ConfigurationErrorsException("type of  RpcLite configuration node can't be null or empty");

						string typeName;
						string assemblyName;

						var splitorIndex = type.IndexOf(",", StringComparison.Ordinal);
						if (splitorIndex > -1)
						{
							typeName = type.Substring(0, splitorIndex);
							assemblyName = type.Substring(splitorIndex + 1);
						}
						else
						{
							typeName = type;
							assemblyName = null;
						}

						var resolver = new ResolverConfigItem
						{
							Name = name,
							Type = type,
							TypeName = typeName,
							AssemblyName = assemblyName,
						};
						Resover = resolver;
					}
				}
			}
			catch (Exception ex)
			{
				throw new ConfigException("Client Configuration Error", ex);
			}
		}

		private void InitializeServiceConfig(XmlNode section)
		{
			try
			{
				var servicesNode = section.SelectSingleNode("services");
				if (servicesNode != null)
				{
					foreach (XmlNode item in servicesNode.ChildNodes)
					{
						if (item.Name != "add" || item.Attributes == null)
							continue;

						var name = item.Attributes.Cast<XmlAttribute>().Where(it => it.Name == "name").Select(it => it.Value).FirstOrDefault();
						var path = item.Attributes.Cast<XmlAttribute>().Where(it => it.Name == "path").Select(it => it.Value).FirstOrDefault();
						var type = item.Attributes.Cast<XmlAttribute>().Where(it => it.Name == "type").Select(it => it.Value).FirstOrDefault();

						if (string.IsNullOrEmpty(name))
							throw new ConfigurationErrorsException("name of  RpcLite configuration node can't be null or empty");
						if (string.IsNullOrEmpty(path))
							throw new ConfigurationErrorsException("path of  RpcLite configuration node can't be null or empty");
						if (string.IsNullOrEmpty(type))
							throw new ConfigurationErrorsException("type of  RpcLite configuration node can't be null or empty");

						string typeName;
						string assemblyName;

						var splitorIndex = type.IndexOf(",", StringComparison.Ordinal);
						if (splitorIndex > -1)
						{
							typeName = type.Substring(0, splitorIndex);
							assemblyName = type.Substring(splitorIndex + 1);
						}
						else
						{
							typeName = type;
							assemblyName = null;
						}

						var serviceConfigItem = new ServiceConfigItem
						{
							Name = name,
							Type = type,
							TypeName = typeName,
							AssemblyName = assemblyName,
							Path = path,
						};
						Services.Add(serviceConfigItem);
					}
				}
			}
			catch (Exception ex)
			{
				throw new ConfigException("Service Configuration Error", ex);
			}
		}

		private static string GetAttribute(string attributeName, XmlNode node)
		{
			if (attributeName == null)
				throw new ArgumentNullException("attributeName");
			if (node == null)
				throw new ArgumentNullException("node");

			return node.Attributes == null
				? null
				: node.Attributes
					.Cast<XmlAttribute>()
					.Where(it => it.Name == attributeName)
					.Select(it => it.Value)
					.FirstOrDefault();
		}

	}
}