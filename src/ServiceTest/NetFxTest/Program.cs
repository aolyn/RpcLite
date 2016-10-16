using System;
using System.IO;
using System.Xml.Serialization;

namespace NetFxTest
{
	class Program
	{
		static void Main(string[] args)
		{
			var ms = new MemoryStream();
			try
			{
				var formatter = new XmlSerializer(typeof(Exception));
				formatter.Serialize(ms, new NotImplementedException("test ex 23432"));
			}
			catch (Exception ex)
			{
			}

			try
			{
				var exObject = new NotImplementedException("test ex 23432");
				var type = exObject.GetType();
				var formatter = new XmlSerializer(typeof(Exception));
				formatter.Serialize(ms, exObject);
				ms.Position = 0;
				var dexObj = formatter.Deserialize(ms);
			}
			catch (Exception ex)
			{
			}

			try
			{
				var exObject = new NotImplementedException("test ex 23432");
				var type = exObject.GetType();
				var formatter = new XmlSerializer(type);
				formatter.Serialize(ms, exObject);
				ms.Position = 0;
				var dexObj = formatter.Deserialize(ms);
			}
			catch (Exception ex)
			{
			}

		}
	}
}
