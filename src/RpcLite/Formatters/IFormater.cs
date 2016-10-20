using System;
using System.Collections.Generic;
using System.IO;

namespace RpcLite.Formatters
{
	/// <summary>
	/// 
	/// </summary>
	public interface IFormatter
	{
		/// <summary>
		/// format name, case sensitive, suggest use lower case, eg: xml, json
		/// </summary>
		string Name { get; }

		/// <summary>
		/// same as HTTP Content-Type, such as application/xml, application/json
		/// </summary>
		List<string> SupportMimes { get; }

		/// <summary>
		/// get if the Formatter can serialize Type of Exception
		/// </summary>
		bool SupportException { get; }

		/// <summary>
		/// 
		/// </summary>
		/// <param name="inputStream"></param>
		/// <param name="targetType"></param>
		/// <returns></returns>
		object Deserialize(Stream inputStream, Type targetType);

		/// <summary>
		/// 
		/// </summary>
		/// <param name="reader"></param>
		/// <param name="targetType"></param>
		/// <returns></returns>
		object Deserialize(TextReader reader, Type targetType);

		/// <summary>
		/// 
		/// </summary>
		/// <param name="outputStream"></param>
		/// <param name="source"></param>
		/// <param name="type"></param>
		void Serialize(Stream outputStream, object source, Type type);

		/// <summary>
		/// 
		/// </summary>
		/// <param name="writer"></param>
		/// <param name="source"></param>
		/// <param name="type"></param>
		void Serialize(TextWriter writer, object source, Type type);

	}
}
