using System;
using RpcLite.Registry;

namespace RpcLite.Config
{
	/// <summary>
	/// ServiceConfigItem
	/// </summary>
	public class ServiceConfigItem
	{
		/// <summary>
		/// name of service
		/// </summary>
		public string Name { get; set; }

		/// <summary>
		/// relative path of service url, eg: ~/api/product/
		/// </summary>
		public string Path { get; set; }

		/// <summary>
		/// relative path of service url, eg: ~/api/product/
		/// </summary>
		public string Address { get; set; }

		/// <summary>
		/// Environment
		/// </summary>
		public string Group { get; set; }

		/// <summary>
		/// original configured type name, eg: ServiceImpl.ProductAsyncService,ServiceImpl
		/// </summary>
		public string Type { get; set; }

		/// <summary>
		/// LifeCycle of service instance
		/// </summary>
		public ServiceLifecycle Lifecycle { get; set; }

		/// <summary>
		/// 
		/// </summary>
		public ServiceConfigItem() { }

		/// <summary>
		/// 
		/// </summary>
		/// <param name="name"></param>
		/// <param name="type"></param>
		/// <param name="path"></param>
		public ServiceConfigItem(string name, Type type, string path)
		{
			Name = name;
			Type = RpcConfigHelper.GetTypeIdentifier(type);
			Path = path;
		}

		/// <summary>
		/// get description string
		/// </summary>
		/// <returns></returns>
		public override string ToString()
		{
			return $"{Name}, { Type }, {Path}";
		}

		/// <summary>
		/// 
		/// </summary>
		/// <returns></returns>
		internal ServiceInfo ToServiceInfo()
		{
			var serviceInfo = new ServiceInfo
			{
				Name = Name,
				Address = Address,
				Group = Group,
			};
			return serviceInfo;
		}
	}
}
