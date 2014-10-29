using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Reflection;
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

		private readonly List<ServiceInfo> _services = new List<ServiceInfo>();
		public List<ServiceInfo> Services
		{
			get { return _services; }
		}

		static RpcLiteConfigSection()
		{
			object sec = ConfigurationManager.GetSection("RpcLite");
			_instance = sec as RpcLiteConfigSection;
		}

		public object Create(object parent, object configContext, XmlNode section)
		{
			if (_instance != null)
				return _instance;

			_instance = new RpcLiteConfigSection();
			try
			{
				foreach (XmlNode item in section.ChildNodes)
				{
					if (item.Name != "add")
						continue;

					var name = item.Attributes.Cast<XmlAttribute>().Where(it => it.Name == "name").Select(it => it.Value).FirstOrDefault();
					var path = item.Attributes.Cast<XmlAttribute>().Where(it => it.Name == "path").Select(it => it.Value).FirstOrDefault();
					var type = item.Attributes.Cast<XmlAttribute>().Where(it => it.Name == "type").Select(it => it.Value).FirstOrDefault();

					if (string.IsNullOrEmpty(path))
						throw new ConfigurationErrorsException("path of  RpcLite configuration node can't be null or empty");
					if (string.IsNullOrEmpty(type))
						throw new ConfigurationErrorsException("type of  RpcLite configuration node can't be null or empty");

					string typeName;
					Assembly assembly;

					int splitorIndex = type.IndexOf(",", StringComparison.Ordinal);
					if (splitorIndex > -1)
					{
						typeName = type.Substring(0, splitorIndex);
						var assemblyName = type.Substring(splitorIndex + 1);
						var asms = AppDomain.CurrentDomain.GetAssemblies();
						assembly = asms.FirstOrDefault(it => it.FullName.StartsWith(assemblyName + ",", StringComparison.OrdinalIgnoreCase))
							?? Assembly.Load(assemblyName);
					}
					else
					{
						typeName = type;
						assembly = Assembly.GetEntryAssembly();
					}

					var typeInfo = assembly.GetType(typeName);

					Services.Add(new ServiceInfo
					{
						Name = name,
						Path = path,
						Type = typeInfo,
					});
				}
			}
			catch (Exception ex)
			{
				throw new ConfigException("Service Configuration Error", ex);
			}

			return this;
		}
	}
}