using System;
using System.Collections.Generic;
using System.IO;

namespace RpcLite.Formatters
{
	/// <summary>
	/// 
	/// </summary>
	public class NetJsonFormater : IFormatter
	{
		/// <summary>
		/// 
		/// </summary>
		public NetJsonFormater()
		{
			SupportMimes.Add("application/json");
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="inputStream"></param>
		/// <param name="targetType"></param>
		/// <returns></returns>
		public object Deserilize(Stream inputStream, Type targetType)
		{
			using (var reader = new StreamReader(inputStream))
			{
				var str = reader.ReadToEnd();
				return NetJSON.NetJSON.Deserialize(targetType, str);
			}
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="outputStream"></param>
		/// <param name="source"></param>
		public void Serialize(Stream outputStream, object source)
		{
			var str = NetJSON.NetJSON.Serialize(source);
			//var writer = new StreamWriter(outputStream);
			//writer.Write(str);
			using (var writer = new StreamWriter(outputStream))
			{
				writer.Write(str);
			}
			//JsonHelper.Serialize(outputStream, source);
		}

		private readonly List<string> _supportMimes = new List<string>();
		/// <summary>
		/// 
		/// </summary>
		public List<string> SupportMimes { get { return _supportMimes; } }
	}
}
