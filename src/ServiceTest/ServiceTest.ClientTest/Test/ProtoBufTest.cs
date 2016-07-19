using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using ProtoBuf;

namespace ServiceTest.ClientTest
{
	public class ProtoBufTest
	{
		public static void Test()
		{
			var p1 = new Person
			{
				Id = 1,
				Name = "八百里开外",
				Address = new Address
				{
					Line1 = "Line1",
					Line2 = "Line2"
				}
			};
			var p2 = new Person
			{
				Id = 2,
				Name = "一枪",
				Address = new Address
				{
					Line1 = "Flat Line1",
					Line2 = "Flat Line2"
				}
			};
			List<Person> pSource = new List<Person>() { p1, p2 };
			string content = ProtobufHelper.Serialize<List<Person>>(pSource);
			Console.Write(content);
			//写入文件
			File.WriteAllText("D://hello.txt", content);
			Console.WriteLine("\r\n****解析部分*****");
			List<Person> pResult = ProtobufHelper.DeSerialize<List<Person>>(content);
			foreach (Person p in pResult)
			{
				Console.WriteLine(p.Name);
			}
			Console.Read();
		}
	}

	[ProtoContract]
	class Person
	{
		[ProtoMember(1)]
		public int Id { get; set; }
		[ProtoMember(2)]
		public string Name { get; set; }
		[ProtoMember(3)]
		public Address Address { get; set; }
	}
	[ProtoContract]
	class Address
	{
		[ProtoMember(1)]
		public string Line1 { get; set; }
		[ProtoMember(2)]
		public string Line2 { get; set; }
	}

	public class ProtobufHelper
	{
		/// <summary>
		/// 序列化
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="t"></param>
		/// <returns></returns>
		public static string Serialize<T>(T t)
		{
			using (MemoryStream ms = new MemoryStream())
			{
				Serializer.Serialize<T>(ms, t);
				return Encoding.UTF8.GetString(ms.ToArray());
			}
		}
		/// <summary>
		/// 反序列化
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="content"></param>
		/// <returns></returns>
		public static T DeSerialize<T>(string content)
		{
			using (MemoryStream ms = new MemoryStream(Encoding.UTF8.GetBytes(content)))
			{
				T t = Serializer.Deserialize<T>(ms);
				return t;
			}
		}
	}

}
