using System;
using System.Collections.Generic;
using System.IO;
using RpcLite.Utility;

namespace RpcLite.Formatters
{
	/// <summary>
	/// 
	/// </summary>
	public class JsonFormatter : IFormatter
	{
		/// <summary>
		/// 
		/// </summary>
		public JsonFormatter()
		{
			SupportMimes.Add("application/json");
		}


		/// <summary>
		/// 
		/// </summary>
		/// <param name="inputStream"></param>
		/// <param name="targetType"></param>
		/// <returns></returns>
		public object Deserialize(Stream inputStream, Type targetType)
		{
			return JsonHelper.Deserialize(inputStream, targetType);
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="reader"></param>
		/// <param name="targetType"></param>
		/// <returns></returns>
		public object Deserialize(TextReader reader, Type targetType)
		{
			throw new NotImplementedException();
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="outputStream"></param>
		/// <param name="source"></param>
		public void Serialize(Stream outputStream, object source)
		{
			var writer = new StreamWriter(outputStream);
			Serialize(writer, source);
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="writer"></param>
		/// <param name="source"></param>
		public void Serialize(TextWriter writer, object source)
		{
			JsonHelper.Serialize(writer, source);
		}

		private readonly List<string> _supportMimes = new List<string>();
		/// <summary>
		/// 
		/// </summary>
		public List<string> SupportMimes { get { return _supportMimes; } }
	}

}
