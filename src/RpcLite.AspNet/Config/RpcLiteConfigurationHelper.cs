using System;
using System.Configuration;
using System.Linq;
using System.Xml;

namespace RpcLite.Config
{
	public class RpcLiteConfigurationHelper
	{
		public static RpcLiteConfig GetConfig(XmlNode section)
		{
			var instance = new RpcLiteConfig();

			var appIdNode = section.SelectSingleNode("appId");
			if (appIdNode != null)
			{
				instance.AppId = appIdNode.InnerText.Trim();
			}

			var envNode = section.SelectSingleNode("environment");
			if (envNode != null)
			{
				instance.Environment = envNode.InnerText.Trim();
			}

			InitializeResolverConfig(section, instance);
			InitializeServiceConfig(section, instance);
			InitializeClientConfig(section, instance);

			return instance;
		}

		// ReSharper disable once FunctionComplexityOverflow
		private static void InitializeClientConfig(XmlNode section, RpcLiteConfig instance)
		{
			var clientsNode = section.SelectSingleNode("clients");
			if (clientsNode != null)
			{
				if (clientsNode.Attributes != null)
				{
					var environment = clientsNode.Attributes["environment"];
					if (environment != null && !string.IsNullOrWhiteSpace(environment.Value))
					{
						instance.ClientEnvironment = environment.Value;
						//_clientEnvironmentAttributeValue = environment.Value;
					}
					else
					{
						instance.ClientEnvironment = instance.Environment;
						//_clientEnvironmentAttributeValue = null;
					}
				}

				foreach (XmlNode item in clientsNode.ChildNodes)
				{
					if (item.Name != "add" || item.Attributes == null)
						continue;

					var name = GetAttribute("name", item);
					var path = GetAttribute("path", item);
					var type = GetAttribute("type", item);
					var nameSpace = GetAttribute("namespace", item);

					if (string.IsNullOrEmpty(name))
						throw new ConfigurationErrorsException("name of RpcLite configuration client node can't be null or empty", item);
					//if (string.IsNullOrEmpty(path))
					//	throw new ConfigurationErrorsException("path of RpcLite configuration node can't be null or empty");
					if (string.IsNullOrEmpty(type))
						throw new ConfigurationErrorsException(string.Format("type of RpcLite configuration client node '{0}' can't be null or empty", name), item);
					if (string.IsNullOrEmpty(nameSpace))
						throw new ConfigurationErrorsException(string.Format("namespace of RpcLite configuration client node '{0}' can't be null or empty", name), item);

					//try
					//{
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
						Namespace = nameSpace,
					};
					instance.Clients.Add(serviceConfigItem);
					//}
					//catch (ConfigurationErrorsException)
					//{
					//	throw;
					//}
					//catch (Exception ex)
					//{
					//	throw new ConfigException("Client Configuration Error", ex);
					//}
				}
			}

		}

		private static void InitializeResolverConfig(XmlNode section, RpcLiteConfig instance)
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
							throw new ConfigurationErrorsException("name of RpcLite configuration addressResolver node can't be null or empty");
						if (string.IsNullOrEmpty(type))
							throw new ConfigurationErrorsException("type of RpcLite configuration addressResolver node " + name + " can't be null or empty");

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
						instance.Resover = resolver;
					}
				}
			}
			catch (Exception ex)
			{
				throw new ConfigException("Client Configuration Error", ex);
			}
		}

		private static void InitializeServiceConfig(XmlNode section, RpcLiteConfig instance)
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
							throw new ConfigurationErrorsException("name of RpcLite service configuration node can't be null or empty");
						if (string.IsNullOrEmpty(path))
							throw new ConfigurationErrorsException("path of RpcLite service configuration node " + name + " can't be null or empty");
						if (string.IsNullOrEmpty(type))
							throw new ConfigurationErrorsException("type of RpcLite service configuration node " + name + " can't be null or empty");

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

						if (string.IsNullOrWhiteSpace(assemblyName))
							throw new ConfigurationErrorsException("assembly can't be null or empty, in RpcLite service configuration node " + name);

						var serviceConfigItem = new ServiceConfigItem
						{
							Name = name,
							Type = type,
							TypeName = typeName,
							AssemblyName = assemblyName,
							Path = path,
						};
						instance.Services.Add(serviceConfigItem);
					}
				}
			}
			catch (Exception ex)
			{
				throw new ConfigException("Service Configuration Error", ex);
			}
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
