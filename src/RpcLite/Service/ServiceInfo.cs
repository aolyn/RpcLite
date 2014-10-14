using System;

namespace RpcLite
{
	public class ServiceInfo
	{
		public string Path { get; set; }
		public Type Type { get; set; }
		public string Name { get; set; }

		public override string ToString()
		{
			return string.Format("{0}, {1}, {2}", Name, Path, Type);
		}
	}
}
