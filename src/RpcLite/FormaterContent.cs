using System;
using System.IO;
using RpcLite.Formatters;

namespace RpcLite
{
	/// <summary>
	/// represents a class to serialize object to stream
	/// </summary>
	public class FormaterContent : IContent
	{
		private readonly IFormatter _formatter;
		private readonly object _content;
		private Type _type;

		/// <summary>
		/// 
		/// </summary>
		/// <param name="formatter"></param>
		/// <param name="content"></param>
		/// <param name="type"></param>
		public FormaterContent(IFormatter formatter, object content, Type type)
		{
			_formatter = formatter;
			_content = content;
			_type = type;
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="stream"></param>
		public void Write(Stream stream)
		{
			_formatter.Serialize(stream, _content, _type);
		}
	}

	/// <summary>
	/// represent a class to write content to stream
	/// </summary>
	public interface IContent
	{
		/// <summary>
		/// write content to stream
		/// </summary>
		/// <param name="stream"></param>
		void Write(Stream stream);
	}
}