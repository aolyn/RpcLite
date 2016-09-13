using System;
using System.Collections.Generic;
using System.Linq;

namespace RpcLite.Config
{
	internal class RpcConfigParserV11 : IRpcConfigParser
	{
		/// <summary>
		/// get RpcLiteConfig from IConfiguration
		/// </summary>
		/// <param name="config"></param>
		/// <returns></returns>
		public RpcConfig GetConfig(IConfiguration config)
		{
			var instance = new RpcConfig
			{
				AppId = config["appId"],
				Environment = config["environment"],
			};

			InitializeServiceConfig(config, instance);
			InitializeClientConfig(config, instance);
			InitializeRegistryConfig(config, instance);
			InitializeMonitorConfig(config, instance);
			InitializeFormatterConfig(config, instance);

			//TODO
			////if (instance.Service?.Paths == null && instance.Service?.Services != null)
			////{
			////	instance.Service.Paths = instance.Service.Services
			////		.Select(it => it.Path)
			////		.ToArray();
			////}

			return instance;
		}

		private static void InitializeFormatterConfig(IConfiguration config, RpcConfig instance)
		{
			var formatterNode = config.GetSection("formatter");

			if (formatterNode?.GetChildren()?.Any() == true)
			{
				var removeDefaultNode = formatterNode["removeDefault"];
				instance.Formatter = new FormatterConfig
				{
					RemoveDefault = removeDefaultNode?.ToLower() == "true",
					Formatters = new List<FormatterItemConfig>(),
				};

				var formattersNode = formatterNode.GetSection("formatters");
				var clients = formattersNode.GetChildren();
				foreach (var item in clients)
				{
					var name = item["name"];
					var type = item["type"];

					//if (string.IsNullOrEmpty(name))
					//	throw new RpcConfigException("name of RpcLite configuration client node can't be null or empty");
					if (string.IsNullOrEmpty(type))
						throw new RpcConfigException(string.Format("type of RpcLite configuration client node '{0}' can't be null or empty", name));

					var serviceConfigItem = new FormatterItemConfig
					{
						Name = name,
						Type = type,
					};
					instance.Formatter.Formatters.Add(serviceConfigItem);
				}
			}
		}

		// ReSharper disable once FunctionComplexityOverflow
		private static void InitializeClientConfig(IConfiguration config, RpcConfig instance)
		{
			var clientNode = config.GetSection("client");
			if (clientNode?.GetChildren()?.Any() != true)
				return;

			instance.Client = new ClientConfig();

			var clusterNode = clientNode.GetSection("cluster");
			if (clusterNode?.GetChildren()?.Any() == true)
			{
				var name = clusterNode["name"];
				var type = clusterNode["type"];

				instance.Client.Cluster = new ClusterConfig
				{
					Name = name,
					Type = type,
				};
			}

			var clientsNode = clientNode.GetSection("clients");
			if (clientsNode?.GetChildren()?.Any() == true)
			{
				var environment = clientsNode["environment"];
				//instance.ClientEnvironment = !string.IsNullOrWhiteSpace(environment) ? environment : instance.Environment;

				var clients = clientsNode.GetChildren();
				foreach (var item in clients)
				{
					var name = item["name"];
					var address = item["address"];
					var type = item["type"];
					var env = item["environment"];
					var nameSpace = item["namespace"];

					if (string.IsNullOrEmpty(name))
						throw new RpcConfigException("name of RpcLite configuration client node can't be null or empty");
					//if (string.IsNullOrEmpty(path))
					//	throw new ConfigurationErrorsException("path of RpcLite configuration node can't be null or empty");
					if (string.IsNullOrEmpty(type))
						throw new RpcConfigException(string.Format("type of RpcLite configuration client node '{0}' can't be null or empty", name));
					if (string.IsNullOrEmpty(nameSpace))
						throw new RpcConfigException(string.Format("namespace of RpcLite configuration client node '{0}' can't be null or empty", name));

					var serviceConfigItem = new ClientConfigItem
					{
						Name = name,
						Type = type,
						//TypeName = typeName,
						//AssemblyName = assemblyName,
						Address = address,
						Namespace = nameSpace,
						Environment = string.IsNullOrWhiteSpace(env) ? environment : env,
					};
					instance.Client.Clients.Add(serviceConfigItem);
				}
			}
		}

		private static void InitializeRegistryConfig(IConfiguration config, RpcConfig instance)
		{
			var registryNode = config.GetSection("registry");
			if (registryNode == null || !registryNode.GetChildren().Any()) return;

			try
			{
				var name = registryNode["name"];
				var type = registryNode["type"];
				var address = registryNode["address"];

				if (string.IsNullOrEmpty(type))
					throw new RpcConfigException("type of RpcLite configuration Registry node " + name + " can't be null or empty");

				var registry = new RegistryConfig
				{
					Name = name,
					Type = type,
					Address = address,
				};
				instance.Registry = registry;
			}
			catch (Exception ex)
			{
				throw new ConfigException("Client Configuration Error", ex);
			}
		}


		private static void InitializeMonitorConfig(IConfiguration config, RpcConfig instance)
		{
			var monitorNode = config.GetSection("monitor");
			if (monitorNode == null || !monitorNode.GetChildren().Any()) return;

			try
			{
				var name = monitorNode["name"];
				var type = monitorNode["type"];
				var address = monitorNode["address"];

				if (string.IsNullOrEmpty(type))
					throw new RpcConfigException("type of RpcLite configuration Monitor node " + name + " can't be null or empty");

				var monitor = new MonitorConfig
				{
					Name = name,
					Type = type,
					Address = address,
				};
				instance.Monitor = monitor;
			}
			catch (Exception ex)
			{
				throw new ConfigException("Client Configuration Error", ex);
			}
		}


		private static void InitializeServiceConfig(IConfiguration config, RpcConfig instance)
		{
			try
			{
				var serviceNode = config.GetSection("service");
				if (serviceNode == null) return;

				if (instance.Service == null)
					instance.Service = new ServiceConfig();

				var servicesNode = serviceNode.GetSection("services");
				if (servicesNode != null)
				{
					var serviceItems = new List<ServiceConfigItem>();
					var serviceItemNodes = servicesNode.GetChildren();
					foreach (var item in serviceItemNodes)
					{
						var name = item["name"];
						var path = item["path"];
						var type = item["type"];
						var address = item["address"];
						var env = item["environment"]; //GetAttribute("type", item);

						if (string.IsNullOrEmpty(name))
							throw new RpcConfigException("name of RpcLite service configuration node can't be null or empty");
						if (string.IsNullOrEmpty(path))
							throw new RpcConfigException("path of RpcLite service configuration node " + name + " can't be null or empty");
						if (string.IsNullOrEmpty(type))
							throw new RpcConfigException("type of RpcLite service configuration node " + name + " can't be null or empty");

						//string typeName;
						//string assemblyName;

						//var splitorIndex = type.IndexOf(",", StringComparison.Ordinal);
						//if (splitorIndex > -1)
						//{
						//	typeName = type.Substring(0, splitorIndex);
						//	assemblyName = type.Substring(splitorIndex + 1);
						//}
						//else
						//{
						//	typeName = type;
						//	assemblyName = null;
						//}

						//if (string.IsNullOrWhiteSpace(assemblyName))
						//	throw new RpcConfigException("assembly can't be null or empty, in RpcLite service configuration node " + name);

						var serviceConfigItem = new ServiceConfigItem
						{
							Name = name,
							Type = type,
							//TypeName = typeName,
							//AssemblyName = assemblyName,
							Path = path,
							Address = address,
							Environment = env,
						};
						serviceItems.Add(serviceConfigItem);
					}
					instance.Service.Services.AddRange(serviceItems);
				}

				var mapperNode = serviceNode.GetSection("mapper");
				if (mapperNode != null)
				{
					instance.Service.Mapper = InitializeServiceMapperConfig(mapperNode);
				}

				var pathsNode = serviceNode.GetSection("paths");
				if (pathsNode != null)
				{
					var paths = new List<string>();
					var pathNodes = pathsNode.GetChildren();
					foreach (var path in pathNodes)
					{
						if (!string.IsNullOrWhiteSpace(path.Value))
							paths.Add(path.Value);
					}
					instance.Service.Paths = paths.ToArray();
				}

			}
			catch (Exception ex)
			{
				throw new ConfigException("Service Configuration Error", ex);
			}
		}

		private static ServiceMapperConfig InitializeServiceMapperConfig(IConfigurationSection monitorNode)
		{
			if (monitorNode == null || !monitorNode.GetChildren().Any())
				return null;

			try
			{
				var name = monitorNode["name"];
				var type = monitorNode["type"];

				if (string.IsNullOrEmpty(type))
					throw new RpcConfigException("type of RpcLite configuration Monitor node " + name + " can't be null or empty");

				return new ServiceMapperConfig
				{
					Name = name,
					Type = type,
				};
			}
			catch (Exception ex)
			{
				throw new ConfigException("Client Configuration Error", ex);
			}
		}

	}
}
