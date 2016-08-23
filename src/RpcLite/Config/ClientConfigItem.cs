using System;

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
		/// namespace
		/// </summary>
		public string Namespace { get; set; }

		/// <summary>
		/// environment
		/// </summary>
		public string Environment { get; set; }

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
		/// 
		/// </summary>
		public ClientConfigItem() { }

		/// <summary>
		/// 
		/// </summary>
		/// <param name="name"></param>
		/// <param name="type"></param>
		/// <param name="address"></param>
		public ClientConfigItem(string name, Type type, string address)
		{
			Name = name;
			Type = RpcConfigHelper.GetTypeIdentifier(type);
			Address = address;
		}


		/// <summary>
		/// extra attribute of service
		/// </summary>
		public string[] Items { get; set; }

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
