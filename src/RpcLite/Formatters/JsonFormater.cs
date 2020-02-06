using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using RpcLite.Utility;

namespace RpcLite.Formatters
{
	/// <inheritdoc />
	/// <summary>
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

		/// <inheritdoc />
		/// <summary>
		/// </summary>
		/// <param name="inputStream"></param>
		/// <param name="targetType"></param>
		/// <returns></returns>
		public object Deserialize(Stream inputStream, Type targetType)
		{
			var reader = new StreamReader(inputStream, Encoding.UTF8);
			return Deserialize(reader, targetType);
		}

		/// <inheritdoc />
		/// <summary>
		/// </summary>
		/// <param name="reader"></param>
		/// <param name="targetType"></param>
		/// <returns></returns>
		public object Deserialize(TextReader reader, Type targetType)
		{
			return JsonHelper.Deserialize(reader, targetType);
		}

		/// <inheritdoc />
		/// <summary>
		/// </summary>
		/// <param name="outputStream"></param>
		/// <param name="source"></param>
		/// <param name="type"></param>
		public void Serialize(Stream outputStream, object source, Type type)
		{
			var writer = new StreamWriter(outputStream, Encoding.UTF8);
			Serialize(writer, source, type);
			writer.Flush();
		}

		/// <inheritdoc />
		/// <summary>
		/// </summary>
		/// <param name="writer"></param>
		/// <param name="source"></param>
		/// <param name="type"></param>
		public void Serialize(TextWriter writer, object source, Type type)
		{
			JsonHelper.Serialize(writer, source);
		}

		/// <inheritdoc />
		/// <summary>
		/// 
		/// </summary>
		public List<string> SupportMimes { get; } = new List<string>();

		/// <inheritdoc />
		/// <summary>
		/// 
		/// </summary>
		public bool SupportException { get; } = true;

		/// <inheritdoc />
		/// <summary>
		/// 
		/// </summary>
		public string Name { get; } = "json";

	}
}
