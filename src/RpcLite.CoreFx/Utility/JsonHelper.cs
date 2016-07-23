#define USE_CUSTOMIZE_JSON_DESERIALIZE

using System;
using System.IO;
using Newtonsoft.Json;
#if USE_CUSTOMIZE_JSON_DESERIALIZE
using RpcLite.Formatters.Json;
#endif

namespace RpcLite.Utility
{
	/// <summary>
	/// </summary>
	public static class JsonHelper
	{
		/// <summary>
		/// </summary>
		/// <param name="stream"></param>
		/// <param name="targetType"></param>
		/// <returns></returns>
		public static object Deserialize(Stream stream, Type targetType)
		{
			using (var reader = new StreamReader(stream))
			{
				return Deserialize(targetType, reader);
			}
		}

#if USE_CUSTOMIZE_JSON_DESERIALIZE
		private static readonly JsonSerializerSettings Settings = new JsonSerializerSettings
		{
			Formatting = Formatting.Indented,
			ContractResolver = new SerializeContractResolver(),
			//Converters = new List<JsonConverter>
			//{
			//	new ExceptionConverter(),
			//	//new EmptyConverter(),
			//	new KeyValuePairConverter(),
			//	new ExpandoObjectConverter()
			//}
		};
#endif

		private static object Deserialize(Type targetType, TextReader reader)
		{
			using (var jsonReader = new JsonTextReader(reader))
			{
				var jsonSerializer = GetSerializer();

				var obj = jsonSerializer.Deserialize(jsonReader, targetType);
				return obj;
			}
		}

#if USE_CUSTOMIZE_JSON_DESERIALIZE
		private static readonly Lazy<JsonSerializer> DefaultJsonSerializer = new Lazy<JsonSerializer>(() =>
		{
			var jsonSerializer = new JsonSerializer
			{
				ContractResolver = Settings.ContractResolver,
			};
			jsonSerializer.Converters.Add(new ExceptionConverter());
			return jsonSerializer;
		});
#else
		private static readonly JsonSerializer DefaultJsonSerializer = new JsonSerializer();
#endif


		private static JsonSerializer GetSerializer()
		{
#if USE_CUSTOMIZE_JSON_DESERIALIZE
			//var jsonSerializer = new JsonSerializer
			//{
			//	ContractResolver = Settings.ContractResolver,
			//};
			//jsonSerializer.Converters.Add(new ExceptionConverter());
			//return jsonSerializer;

			return DefaultJsonSerializer.Value;
#else
			//return new JsonSerializer();
			return DefaultJsonSerializer;
#endif

		}

		/// <summary>
		/// </summary>
		public static string Serialize(object source)
		{
			using (var writer = new StringWriter())
			{
				Serialize(source, writer);
				writer.Flush();
				return writer.ToString();
			}
		}

		/// <summary>
		/// </summary>
		public static void Serialize(Stream stream, object source)
		{
			//using (var writer = new StreamWriter(stream))
			//{
			var writer = new StreamWriter(stream);
			Serialize(source, writer);
			//writer.Flush();
			//}
		}

		private static void Serialize(object source, TextWriter writer)
		{
			//using (var jsonWriter = new JsonTextWriter(writer))
			//{
			var jsonWriter = new JsonTextWriter(writer);
			//var jsonSerializer = new JsonSerializer
			//{
			//	NullValueHandling = NullValueHandling.Ignore
			//};

			var jsonSerializer = GetSerializer();

			jsonSerializer.Serialize(jsonWriter, source);
			writer.Flush();
			//}
		}
	}
}