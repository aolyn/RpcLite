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
		/// <param name="leaveOpen"></param>
		/// <returns></returns>
		public static object Deserialize(Stream stream, Type targetType, bool leaveOpen = true)
		{
			//using (var reader = new StreamReader(stream, Encoding.UTF8, true, 1024, leaveOpen))
			var reader = new StreamReader(stream);
			return Deserialize(reader, targetType, leaveOpen);
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="reader"></param>
		/// <param name="targetType"></param>
		/// <param name="leaveOpen"></param>
		/// <returns></returns>
		public static object Deserialize(TextReader reader, Type targetType, bool leaveOpen = true)
		{
			using (var jsonReader = new JsonTextReader(reader))
			{
				jsonReader.CloseInput = !leaveOpen;

				var jsonSerializer = GetSerializer();
				//var jsonSerializer = JsonSerializer.Create();
				var obj = jsonSerializer.Deserialize(jsonReader, targetType);
				return obj;
			}
		}

#if USE_CUSTOMIZE_JSON_DESERIALIZE
		//private static readonly Lazy<JsonSerializer> DefaultJsonSerializer = new Lazy<JsonSerializer>(() =>
		//{
		//	var jsonSerializer = new JsonSerializer
		//	{
		//		ContractResolver = Settings.ContractResolver,
		//	};
		//	jsonSerializer.Converters.Add(new ExceptionConverter());
		//	return jsonSerializer;
		//});
#else
		private static readonly JsonSerializer DefaultJsonSerializer = new JsonSerializer();
#endif


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

		private static JsonSerializer GetSerializer()
		{
#if USE_CUSTOMIZE_JSON_DESERIALIZE
			//var jsonSerializer = new JsonSerializer
			//{
			//	ContractResolver = Settings.ContractResolver,
			//};
			var jsonSerializer = JsonSerializer.Create();
			jsonSerializer.ContractResolver = Settings.ContractResolver;
			jsonSerializer.Converters.Add(new ExceptionConverter());

			return jsonSerializer;

			//return DefaultJsonSerializer.Value;
#else
			return new JsonSerializer();
			//return DefaultJsonSerializer;
#endif

		}

		/// <summary>
		/// </summary>
		public static string Serialize(object source)
		{
			using (var writer = new StringWriter())
			{
				Serialize(writer, source);
				writer.Flush();
				return writer.ToString();
			}
		}

		/// <summary>
		/// </summary>
		public static void Serialize(Stream stream, object source)
		{
			var writer = new StreamWriter(stream);
			Serialize(writer, source);
			writer.Flush();
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="writer"></param>
		/// <param name="source"></param>
		public static void Serialize(TextWriter writer, object source)
		{
			using (var jsonWriter = new JsonTextWriter(writer))
			{
				jsonWriter.CloseOutput = false;
				var jsonSerializer = GetSerializer();
				jsonSerializer.Serialize(jsonWriter, source);
			}
		}

	}
}