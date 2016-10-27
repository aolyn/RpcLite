using System;
using System.Xml;

namespace NuGetPackTool
{
	public class NuspecHelper
	{
		public static string GetVerson(string file)
		{
			var xml = new XmlDocument();
			xml.Load(file);

			var xmlnsm = new XmlNamespaceManager(xml.NameTable);
			if (xml.DocumentElement?.NamespaceURI == null)
				return null;

			xmlnsm.AddNamespace("def", xml.DocumentElement?.NamespaceURI);

			var versionNode = xml.DocumentElement?.SelectSingleNode("/def:package/def:metadata/def:version", xmlnsm);
			var version = versionNode?.InnerText;
			return version;
		}

		public static void SetVersion(string file, string version)
		{
			var xml = new XmlDocument();
			xml.Load(file);

			var xmlnsm = new XmlNamespaceManager(xml.NameTable);
			if (xml.DocumentElement?.NamespaceURI == null)
				return;
			xmlnsm.AddNamespace("def", xml.DocumentElement?.NamespaceURI);
			//xmlnsm.DefaultNamespace = xml.DocumentElement?.Name;

			var versionNode = xml.DocumentElement?.SelectSingleNode("/def:package/def:metadata/def:version", xmlnsm);

			if (versionNode != null)
			{
				versionNode.InnerText = version;
			}

			xml.Save(file);
		}

		public static void AddOneToRevisionVersion(string file)
		{
			var version = GetVerson(file);
			if (version == null)
				return;

			var ver = new Version(version);
			var newVersion = new Version(ver.Major, ver.Minor, ver.Build, ver.Revision + 1);
			SetVersion(file, newVersion.ToString());
		}

	}
}
