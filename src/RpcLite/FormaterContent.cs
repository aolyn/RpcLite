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

		/// <summary>
		/// 
		/// </summary>
		/// <param name="formatter"></param>
		/// <param name="content"></param>
		public FormaterContent(IFormatter formatter, object content)
		{
			_formatter = formatter;
			_content = content;
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="stream"></param>
		public void Write(Stream stream)
		{
			_formatter.Serialize(stream, _content);
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