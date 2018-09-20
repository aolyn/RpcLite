using System;
using System.Collections.Generic;

namespace RpcLite.Config
{
	/// <summary>
	/// client configuration item
	/// </summary>
	public class ClientConfigItem
	{
		private string _type;

		/// <summary>
		/// name of service
		/// </summary>
		public string Name { get; set; }

		/// <summary>
		/// environment
		/// </summary>
		public string Group { get; set; }

		/// <summary>
		/// service url, eg: http://localhost/api/product
		/// </summary>
		public string Address { get; set; }

		/// <summary>
		/// assembly of service implement class
		/// </summary>
		public string AssemblyName { get; private set; }

		/// <summary>
		/// full service type name, eg: ServiceImpl.ProductAsyncService
		/// </summary>
		public string TypeName { get; private set; }

		///// <summary>
		///// LifeCycle of DI instance
		///// </summary>
		//public ServiceLifecycle Lifecycle { get; set; }

		/// <summary>
		/// original configured type name, eg: ServiceImpl.ProductAsyncService,ServiceImpl
		/// </summary>
		public string Type
		{
			get { return _type; }
			set
			{
				TypeName = null;
				AssemblyName = null;
				_type = value;
				var segs = _type?.Split(',');
				if (segs?.Length == 2)
				{
					TypeName = segs[0].Trim();
					AssemblyName = segs[1].Trim();
				}
			}
		}

		/// <summary>
		/// raw configuration items (key/value)
		/// </summary>
		public IDictionary<string, string> Items { get; set; }

		/// <summary>
		/// 
		/// </summary>
		public ClientConfigItem() { }

		/// <summary>
		/// 
		/// </summary>
		/// <param name="name">service name</param>
		/// <param name="type">contract type</param>
		/// <param name="address"></param>
		public ClientConfigItem(string name, Type type, string address)
		{
			Name = name;
			Type = RpcConfigHelper.GetTypeIdentifier(type);
			Address = address;
		}

		///// <summary>
		///// extra attribute of service
		///// </summary>
		//public string[] Items { get; set; }

		/// <summary>
		/// get description string
		/// </summary>
		/// <returns></returns>
		public override string ToString()
		{
			return $"{Name}, { Type }, {Address}";
		}
	}
}
