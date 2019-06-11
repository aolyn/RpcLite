using System;

namespace RpcLite.Config
{
	internal class RpcConfigParserV1 : IRpcConfigParser
	{
		/// <summary>
		/// get RpcLiteConfig from IConfiguration
		/// </summary>
		/// <param name="config"></param>
		/// <returns></returns>
		public RpcConfig GetConfig(IRpcConfiguration config)
		{
			var instance = new RpcConfig
			{
				AppId = config["appId"],
				Group = config["environment"],
				Version = RpcConfigHelper.GetVersion(config),
			};

			//InitializeResolverConfig(config, instance);
			InitializeServiceConfig(config, instance);
			InitializeClientConfig(config, instance);

			return instance;
		}

		// ReSharper disable once FunctionComplexityOverflow
		private static void InitializeClientConfig(IRpcConfiguration config, RpcConfig instance)
		{
			var clientsNode = config.GetSection("clients");
			if (clientsNode != null)
			{
				var environment = clientsNode["environment"];
				//instance.ClientEnvironment = !string.IsNullOrWhiteSpace(environment) ? environment : instance.Environment;
				if (instance.Client == null) instance.Client = new ClientConfig();

				var clients = clientsNode.GetChildren();
				foreach (var item in clients)
				{
					var name = item["name"]; // GetAttribute("name", item);
					var address = item["address"]; //GetAttribute("path", item);
					var type = item["type"]; //GetAttribute("type", item);
					var env = item["environment"]; //GetAttribute("type", item);
					//var nameSpace = item["namespace"]; // GetAttribute("namespace", item);

					if (string.IsNullOrEmpty(name))
						throw new RpcConfigException("name of RpcLite configuration client node can't be null or empty");
					//if (string.IsNullOrEmpty(path))
					//	throw new ConfigurationErrorsException("path of RpcLite configuration node can't be null or empty");
					if (string.IsNullOrEmpty(type))
						throw new RpcConfigException(string.Format("type of RpcLite configuration client node '{0}' can't be null or empty", name));
					//if (string.IsNullOrEmpty(nameSpace))
					//	throw new RpcConfigException(string.Format("namespace of RpcLite configuration client node '{0}' can't be null or empty", name));

					var serviceConfigItem = new ClientConfigItem
					{
						Name = name,
						Type = type,
						//TypeName = typeName,
						//AssemblyName = assemblyName,
						Address = address,
						//Namespace = nameSpace,
						Group = string.IsNullOrWhiteSpace(env) ? environment : env,
					};
					instance.Client.Clients.Add(serviceConfigItem);
				}
			}
		}

		//private static void InitializeResolverConfig(IConfiguration config, RpcConfig instance)
		//{
		//	try
		//	{
		//		var resolverNode = config.GetSection("addressResolver");
		//		if (resolverNode != null && resolverNode.GetChildren().Any())
		//		{
		//			//foreach (XmlNode item in resolverNode.ChildNodes)
		//			{
		//				//if (resolverNode.Name != "add" || resolverNode.Attributes == null)
		//				//	continue;

		//				var name = resolverNode["name"]; //GetAttribute("name", resolverNode);
		//				var type = resolverNode["type"]; //GetAttribute("type", resolverNode);

		//				if (string.IsNullOrEmpty(name))
		//					throw new RpcConfigException("name of RpcLite configuration addressResolver node can't be null or empty");
		//				if (string.IsNullOrEmpty(type))
		//					throw new RpcConfigException("type of RpcLite configuration addressResolver node " + name + " can't be null or empty");

		//				string typeName;
		//				string assemblyName;

		//				var splitorIndex = type.IndexOf(",", StringComparison.Ordinal);
		//				if (splitorIndex > -1)
		//				{
		//					typeName = type.Substring(0, splitorIndex);
		//					assemblyName = type.Substring(splitorIndex + 1);
		//				}
		//				else
		//				{
		//					typeName = type;
		//					assemblyName = null;
		//				}

		//				var resolver = new ResolverConfigItem
		//				{
		//					Name = name,
		//					Type = type,
		//					TypeName = typeName,
		//					AssemblyName = assemblyName,
		//				};
		//				instance.Resover = resolver;
		//			}
		//		}
		//	}
		//	catch (Exception ex)
		//	{
		//		throw new ConfigException("Client Configuration Error", ex);
		//	}
		//}

		private static void InitializeServiceConfig(IRpcConfiguration config, RpcConfig instance)
		{
			try
			{
				var servicesNode = config.GetSection("services");
				if (servicesNode != null)
				{
					instance.Service = instance.Service ?? new ServiceConfig();
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
							Group = env,
						};
						instance.Service.Services.Add(serviceConfigItem);
					}
				}
			}
			catch (Exception ex)
			{
				throw new ConfigException("Service Configuration Error", ex);
			}
		}
	}
}
