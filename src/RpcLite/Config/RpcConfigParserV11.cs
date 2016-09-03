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

			if (instance.Service.Services != null)
			{
				instance.ServicePaths = instance.Service.Services
					.Select(it => it.Path)
					.ToArray();
			}

			return instance;
		}

		// ReSharper disable once FunctionComplexityOverflow
		private static void InitializeClientConfig(IConfiguration config, RpcConfig instance)
		{
			var clientNode = config.GetSection("client");

			var clientsNode = clientNode?.GetSection("clients");
			if (clientsNode != null)
			{
				var environment = clientsNode["environment"];
				//instance.ClientEnvironment = !string.IsNullOrWhiteSpace(environment) ? environment : instance.Environment;

				instance.Client = new ClientConfig();
				var clients = clientsNode.GetChildren();
				foreach (var item in clients)
				{
					var name = item["name"]; // GetAttribute("name", item);
					var address = item["address"]; //GetAttribute("path", item);
					var type = item["type"]; //GetAttribute("type", item);
					var env = item["environment"]; //GetAttribute("type", item);
					var nameSpace = item["namespace"]; // GetAttribute("namespace", item);

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
				if (servicesNode != null)
				{
					instance.Service.Mapper = InitializeServiceMapperConfig(mapperNode);
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
