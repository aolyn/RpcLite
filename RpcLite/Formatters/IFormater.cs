using System;
using System.Collections.Generic;
using System.IO;

namespace RpcLite.Formatters
{
	public interface IFormatter
	{
		object Deserilize(Stream inputStream,Type targetType);
		void Serialize(Stream outputStream, object source);
		List<string> SupportMimes { get; }
	}
}
