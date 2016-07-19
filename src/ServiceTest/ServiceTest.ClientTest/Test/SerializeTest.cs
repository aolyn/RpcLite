using System;
using System.IO;
using System.Text;
using RpcLite.Utility;

namespace ServiceTest.ClientTest.Test
{
	public class SerializeTest
	{
		internal static void PropertyReflectTest()
		{
			{
				var fGetter = PropertyReflector.MakeObjectFieldGetter(PropertyReflector.GetField(typeof(MyClass), "Name"));
				var fSetter = PropertyReflector.MakeObjectFieldSetter(PropertyReflector.GetField(typeof(MyClass), "Name"));

				var pGetter = PropertyReflector.MakeObjectPropertyGetter(PropertyReflector.GetProperty(typeof(MyClass), "Weight"));
				var pSetter = PropertyReflector.MakeObjectPropertySetter(PropertyReflector.GetProperty(typeof(MyClass), "Weight"));

				try
				{
					var mc1 = new MyClass();

					var name = "Chris";
					fSetter(mc1, name);
					var rname = fGetter(mc1);
					if (name != (string)rname)
					{
						throw new Exception();
					}

					var weight = 62;
					pSetter(mc1, weight);
					var rweight = (int)pGetter(mc1);
					if (weight != rweight)
					{
						throw new Exception();
					}
				}
				catch (Exception ex)
				{
					Console.WriteLine("****** PropertyReflectTest() test failed");
					throw;
				}

			}

			{
				var fGetter = PropertyReflector<MyClass, string>.GetFieldGetterFunc("Name");
				var fSetter = PropertyReflector<MyClass, string>.GetFieldSetterFunc("Name");

				var pGetter = PropertyReflector<MyClass, int>.GetPropertyGetterFunc("Weight");
				var pSetter = PropertyReflector<MyClass, int>.GetPropertySetterFunc("Weight");

				try
				{
					var mc1 = new MyClass();

					var name = "Chris";
					fSetter(mc1, name);
					var rname = fGetter(mc1);
					if (name != rname)
					{
						throw new Exception();
					}

					var weight = 62;
					pSetter(mc1, weight);
					var rweight = pGetter(mc1);
					if (weight != rweight)
					{
						throw new Exception();
					}
				}
				catch (Exception ex)
				{
					Console.WriteLine("****** PropertyReflectTest() generic test failed");
					throw;
				}

			}
		}

		internal static void ExceptionSerializationTest()
		{
			NotImplementedException exobj;
			try
			{
				throw new NotImplementedException("test exception 22222222222");
			}
			catch (NotImplementedException ex)
			{
				exobj = ex;
			}
			var ms = new MemoryStream();
			JsonHelper.Serialize(ms, exobj);
			var json = Encoding.UTF8.GetString(ms.ToArray());

			ms.Position = 0;
			var dexobj = JsonHelper.Deserialize(ms, exobj.GetType());
		}


		private static void Test2()
		{
#if NETCORE

			//ProtoBufTest.Test();
			{
				object exobj = null;
				try
				{
					var b = 3;
					b = 0;
					var c = 2 / b;
				}
				catch (Exception exs)
				{
					exobj = exs;
				}
				//var ex = new Exception("test ex");
				var ex = exobj;
				//var ex = new ServiceTest.Contract.Product
				//{
				//	Id = 223,
				//	Name = "abc book",
				//	Category = "Book",
				//	ListDate = DateTime.Now,
				//	Price = 34,
				//	Tags = new List<string>
				//	{
				//		"book",
				//		"tech",
				//		"new"
				//	}
				//};

				{
					try
					{
						var setting = new JsonSerializerSettings
						{
							Formatting = Formatting.Indented,
							ContractResolver = new SerializeContractResolver()
						};
						var json = JsonConvert.SerializeObject(ex, setting);
						Console.WriteLine(json);
						var dex = JsonConvert.DeserializeObject(json, ex.GetType(), setting);
					}
					catch (Exception)
					{
						throw;
					}
				}

				var dcs = new DataContractSerializer(ex.GetType());
				var ms = new MemoryStream();
				dcs.WriteObject(ms, ex);

				ms.Position = 0;
				var dex3 = dcs.ReadObject(ms);
				var xml = Encoding.UTF8.GetString(ms.ToArray());

				var jss = new System.Runtime.Serialization.Json.DataContractJsonSerializer(ex.GetType());
				var jsms = new MemoryStream();
				jss.WriteObject(jsms, ex);

				ms.Position = 0;
				var dexjs = dcs.ReadObject(ms);
				var jsss = Encoding.UTF8.GetString(ms.ToArray());

				var product = new ServiceTest.Contract.Product
				{
					Id = 223,
					Name = "abc book",
					Category = "Book",
					ListDate = DateTime.Now,
					Price = 34,
					Tags = new List<string>
					{
						"book",
						"tech",
						"new"
					}
				};
				var xmlSeriaizer = new System.Xml.Serialization.XmlSerializer(product.GetType());
				var stringWriter = new StringWriter();
				//var xmlWriter = new XmlWriter();
				xmlSeriaizer.Serialize(stringWriter, (object)product);
			}
#endif
		}

		public static void SetName(MyClass obj, string name)
		{
			obj.Name = name;
		}

		public static void SetAge(MyClass obj, int age)
		{
			obj.Age = age;
		}

		public static int GetWeight(MyClass obj)
		{
			return obj.Weight;
		}

		public static object GetWeight2(object obj)
		{
			return ((MyClass)obj).Weight;
		}

		public static void SetAge2(object obj, object age)
		{
			((MyClass)obj).Age = (int)age;
		}

	}

	public class MyClass
	{
		public string Name;

		public int Age { get; set; }
		public int Weight { get; set; }
	}

}
