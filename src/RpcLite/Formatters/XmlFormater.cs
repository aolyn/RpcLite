using System;
using System.Collections.Generic;
using System.IO;
using System.Xml.Serialization;

namespace RpcLite.Formatters
{

	/// <summary>
	/// 
	/// </summary>
	public class XmlFormatter : IFormatter
	{
		/// <summary>
		/// 
		/// </summary>
		public XmlFormatter()
		{
			SupportMimes.Add("text/xml");
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="inputStream"></param>
		/// <param name="targetType"></param>
		/// <returns></returns>
		public object Deserilize(Stream inputStream, Type targetType)
		{
			var serializer = new XmlSerializer(targetType);
			return serializer.Deserialize(inputStream);
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="outputStream"></param>
		/// <param name="source"></param>
		public void Serialize(Stream outputStream, object source)
		{
			var serializer = new XmlSerializer(source.GetType());
			serializer.Serialize(outputStream, source);
		}

		private readonly List<string> _supportMimes = new List<string>();
		/// <summary>
		/// 
		/// </summary>
		public List<string> SupportMimes { get { return _supportMimes; } }
	}
}
