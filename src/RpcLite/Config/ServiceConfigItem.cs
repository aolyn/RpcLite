using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RpcLite.Config
{
	public class ServiceConfigItem
	{
		public string Path { get; set; }

		public string AssemblyName { get; set; }

		public string TypeName { get; set; }

		public string Type { get; set; }

		public string Name { get; set; }
	}
}
