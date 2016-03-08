using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml.Serialization;
using Model;
using Newtonsoft.Json;

namespace WebApiClient
{
	class SerializationTest
	{
		public static void Test()
		{
			var obj = CreateObject(typeof(NewTestModelClass));
			var converter = new RpcListJsonConverter();
			var settings = new JsonSerializerSettings
			{
				DateFormatHandling = DateFormatHandling.IsoDateFormat,
				NullValueHandling = NullValueHandling.Ignore,
				DefaultValueHandling = DefaultValueHandling.Ignore,
			};

			var str = JsonConvert.SerializeObject(obj, Formatting.Indented, settings);
			var str32 = JsonConvert.SerializeObject(obj, Formatting.Indented);

			{
				var product = new Model.Product
				{
					Id = 1,
					Category = DateTime.Now.ToString(CultureInfo.CurrentCulture),
					Name = DateTime.Now.ToString(CultureInfo.CurrentCulture),
					Price = 123123,
					ListDate = DateTime.Now,
				};

				var stream = new StringWriter();
				var xmlSerializer = new XmlSerializer(product.GetType());
				xmlSerializer.Serialize(stream, product);
				var result = stream.ToString();
			}

			//{
			//	var ex = new Exception("TEST");
			//	var je = JsonConvert.SerializeObject(ex);
			//	var jo = JsonConvert.DeserializeObject<Exception>(je);

			//	var exbs = Encoding.UTF8.GetBytes(je);
			//	var msEx = new MemoryStream(exbs);
			//	TextReader sr = new StringReader(je);
			//	sr = new StreamReader(msEx);
			//	var jr = new JsonTextReader(sr);
			//	var jc = new JsonSerializer();
			//	var o = jc.Deserialize(jr, ex.GetType());

			//	je = "{\"Message\":\"An error has occurred.\",\"ExceptionMessage\":\"TEST EXCEPTION\",\"ExceptionType\":\"System.Exception\",\"StackTrace\":\"   at WebApiTest.ProductController.Get(Int32 id) in e:\\Documents\\Visual Studio 2010\\Projects\\WebApiTest\\WebApiTest\\Controllers\\ProductsController.cs:line 38\r\n   at lambda_method(Closure , Object , Object[] )\r\n   at System.Web.Http.Controllers.ReflectedHttpActionDescriptor.ActionExecutor.<>c__DisplayClass13.<GetExecutor>b__c(Object instance, Object[] methodParameters)\r\n   at System.Web.Http.Controllers.ReflectedHttpActionDescriptor.ActionExecutor.Execute(Object instance, Object[] arguments)\r\n   at System.Web.Http.Controllers.ReflectedHttpActionDescriptor.<>c__DisplayClass5.<ExecuteAsync>b__4()\r\n   at System.Threading.Tasks.TaskHelpers.RunSynchronously[TResult](Func`1 func, CancellationToken cancellationToken)\"}";
			//	var ser = new JavaScriptSerializer();
			//	var jo2 = JsonConvert.DeserializeObject(je);
			//}

		}


		public static object CreateObject(Type type)
		{
			return CreateObject(type, null);
		}

		public static object CreateObject(Type type, string propertName)
		{
			if (type == typeof(long)
				|| type == typeof(int)
				|| type == typeof(uint))
			{
				return new Random().Next(99999);
			}
			else if (type == typeof(decimal))
			{
				return (decimal)new Random().Next(99999);
			}
			else if (type == typeof(string))
			{
				return propertName + " string";
			}
			else if (type == typeof(DateTime))
			{
				return DateTime.Now;
			}
			else if (type == typeof(Guid))
			{
				return Guid.NewGuid();
			}
			else if (type.IsValueType)
			{
				var value = Activator.CreateInstance(type);
				return value;
			}
			else if (type.IsClass)
			{
				if (type.IsGenericType)
				{
					var gt = type.GetGenericTypeDefinition();
					if (gt == typeof(List<>))
					{
						var list = Activator.CreateInstance(type);
						for (int i = 0; i < 4; i++)
						{
							var element = CreateObject(type.GetGenericArguments()[0], "item");
							var addMethod = type.GetMethod("Add");
							addMethod.Invoke(list, new object[] { element });
						}
						return list;
					}
					else
					{

					}
				}

				var obj = Activator.CreateInstance(type);
				var propeties = type.GetProperties();
				foreach (var property in propeties)
				{
					var value = CreateObject(property.PropertyType, property.Name);
					property.SetValue(obj, value, new object[0]);
				}
				return obj;
				//property.SetValue(obj, value, new object[0]);
			}
			else
			{

			}

			return null;
		}

		struct Point
		{
			public int X { get; set; }
			public int Y { get; set; }
		}


		class Person
		{
			public int Age { get; set; }
			public Point Point { get; set; }
		}

		class NewTestModelClass
		{
			public int Id { get; set; }
			public int Name { get; set; }
			public Product Product { get; set; }
			public DateTime DateCreated { get; set; }
			public List<Product> Products { get; set; }
			public Person Person { get; set; }

		}

		class RpcListJsonConverter : JsonConverter
		{
			public override bool CanConvert(Type objectType)
			{
				if (objectType.IsPrimitive
					|| objectType == typeof(int))
				{
					return true;
				}

				return false;
			}

			//public override Newtonsoft.Json.Schema.JsonSchema GetSchema()
			//{
			//	return base.GetSchema();
			//}

			public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
			{
				return null;
			}

			public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
			{
				//writer.WriteStartObject();
				//writer.WritePropertyName("");
				writer.WriteValue(value);
				//writer.WriteEndObject();
			}
		}

		//public class DataSetConverter : JsonConverter
		//{
		//	public override bool CanConvert(Type objectType)
		//	{
		//		return typeof(DataSet).IsAssignableFrom(objectType);
		//	}

		//	public override void WriteJson(JsonWriter writer, object value)
		//	{
		//		DataSet ds = (DataSet)value;
		//		writer.WriteStartObject();
		//		foreach (DataTable dt in ds.Tables)
		//		{
		//			writer.WritePropertyName(dt.TableName);
		//			writer.WriteStartArray();
		//			foreach (DataRow dr in dt.Rows)
		//			{
		//				writer.WriteStartObject();
		//				foreach (DataColumn dc in dt.Columns)
		//				{
		//					writer.WritePropertyName(dc.ColumnName);
		//					writer.WriteValue(dr[dc].ToString());
		//				}
		//				writer.WriteEndObject();
		//			}
		//			writer.WriteEndArray();
		//		}
		//		writer.WriteEndObject();
		//	}
		//}
	}
}
