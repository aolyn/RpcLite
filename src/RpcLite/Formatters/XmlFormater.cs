using System;
using System.Collections.Generic;
using System.IO;

namespace RpcLite.Formatters
{
	public class XmlFormatter : IFormatter
	{
		public XmlFormatter()
		{
			SupportMimes.Add("text/xml");
		}

		public object Deserilize(Stream inputStream, Type targetType)
		{
			throw new NotImplementedException();
		}

		public void Serialize(Stream outputStream, object source)
		{
			throw new NotImplementedException();
		}

		private readonly List<string> _supportMimes = new List<string>();
		public List<string> SupportMimes { get { return _supportMimes; } }
	}
}
