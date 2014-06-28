using System;
using System.Collections.Generic;
using System.IO;
using RpcLite.Utility;

namespace RpcLite.Formatters
{
	public class JsonFormatter : IFormatter
	{
		public JsonFormatter()
		{
			SupportMimes.Add("application/json");
		}

		public object Deserilize(Stream inputStream, Type targetType)
		{
			return JsonHelper.Deserialize(inputStream, targetType);
		}

		public void Serialize(Stream outputStream, object source)
		{
			JsonHelper.Serialize(outputStream, source);
		}

		private readonly List<string> _supportMimes = new List<string>();
		public List<string> SupportMimes { get { return _supportMimes; } }
	}
}
