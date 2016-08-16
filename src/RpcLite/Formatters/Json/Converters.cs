using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Serialization;
using RpcLite.Logging;
using RpcLite.Utility;

namespace RpcLite.Formatters.Json
{
	/// <summary>
	/// 
	/// </summary>
	public class ExceptionConverter : CustomCreationConverter<Exception>
	{
		private string _className;

		/// <summary>
		/// 
		/// </summary>
		/// <param name="reader"></param>
		/// <param name="objectType"></param>
		/// <param name="existingValue"></param>
		/// <param name="serializer"></param>
		/// <returns></returns>
		public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
		{
			if (reader.TokenType == JsonToken.Null)
			{
				return base.ReadJson(reader, objectType, existingValue, serializer);
			}

			var stateQueue = new Queue<JsonReaderState>(2);

			var state0 = GetState(reader);
			stateQueue.Enqueue(state0);

			if (state0.TokenType == JsonToken.StartObject)
			{
				reader.Read();
				var state1 = GetState(reader);
				stateQueue.Enqueue(state1);

				if (state1.ValueType == typeof(string) && (string)state1.Value == "ClassName")
				{
					reader.Read();
					var state2 = GetState(reader);
					stateQueue.Enqueue(state2);
					_className = (string)reader.Value;
				}
			}

			var newReader = new BufferJsonReader(reader, stateQueue);

			try
			{
				var obj = base.ReadJson(newReader, objectType, existingValue, serializer);
				return obj;
			}
			catch (Exception ex)
			{
				LogHelper.Debug(ex.ToString());
				throw;
			}

		}

		private static JsonReaderState GetState(JsonReader reader)
		{
			return new JsonReaderState
			{
				TokenType = reader.TokenType,
				ValueType = reader.ValueType,
				Value = reader.Value,
				Path = reader.Path,
				Depth = reader.Depth,
				QuoteChar = reader.QuoteChar,
			};
		}

		/// <summary>
		/// Creates an object which will then be populated by the serializer.
		/// </summary>
		/// <param name="objectType">Type of the object.</param>
		/// <returns>The created object.</returns>
		public override Exception Create(Type objectType)
		{
			if (_className == null)
				throw new Exception("ClassName is null");

			var type = objectType.FullName != _className
				? Type.GetType(_className)
				: objectType;

			type = type ?? typeof(Exception);

			return (Exception)Activator.CreateInstance(type);
		}
	}

	/// <summary>
	/// 
	/// </summary>
	public class JsonReaderState
	{
		/// <summary>
		/// 
		/// </summary>
		public JsonToken TokenType { get; internal set; }
		/// <summary>
		/// 
		/// </summary>
		public Type ValueType { get; internal set; }
		/// <summary>
		/// 
		/// </summary>
		public object Value { get; internal set; }
		/// <summary>
		/// 
		/// </summary>
		public string Path { get; internal set; }
		/// <summary>
		/// 
		/// </summary>
		public int Depth { get; internal set; }
		/// <summary>
		/// 
		/// </summary>
		public char QuoteChar { get; internal set; }
	}

	/// <summary>
	/// 
	/// </summary>
	public class BufferJsonReader : JsonReader
	{
		private readonly Queue<JsonReaderState> _stateQueue;
		private readonly JsonReader _reader;
		private JsonReaderState _state;

		/// <summary>
		/// 
		/// </summary>
		public BufferJsonReader(JsonReader reader)
		{
			_reader = reader;
		}

		/// <summary>
		/// 
		/// </summary>
		public override JsonToken TokenType { get { return _state != null ? _state.TokenType : _reader.TokenType; } }

		/// <summary>
		/// 
		/// </summary>
		public override Type ValueType { get { return _state != null ? _state.ValueType : _reader.ValueType; } }

		/// <summary>
		/// 
		/// </summary>
		public override object Value { get { return _state != null ? _state.Value : _reader.Value; } }

		/// <summary>
		/// 
		/// </summary>
		public override string Path { get { return _state != null ? _state.Path : _reader.Path; } }

		/// <summary>
		/// 
		/// </summary>
		public override int Depth { get { return _state != null ? _state.Depth : _reader.Depth; } }

		/// <summary>
		/// 
		/// </summary>
		public override char QuoteChar { get { return _state != null ? _state.QuoteChar : _reader.QuoteChar; } }

		/// <summary>
		/// 
		/// </summary>
		public override void Close()
		{
			_reader.Close();
		}

		/// <summary>
		/// 
		/// </summary>
		public override string ReadAsString()
		{
			var value = base.ReadAsString();
			return value;
			//return base.ReadAsString();
		}

		/// <summary>
		/// 
		/// </summary>
		public override int? ReadAsInt32()
		{
			if (!Read())
			{
				return null;
			}

			return Convert.ToInt32(Value);

			//var value = base.ReadAsInt32();
			//return value;
		}

		/// <summary>
		/// 
		/// </summary>
		public BufferJsonReader(JsonReader reader, Queue<JsonReaderState> stateQueue) : this(reader)
		{
			_reader = reader;
			if (stateQueue != null && stateQueue.Count > 0)
			{
				_stateQueue = new Queue<JsonReaderState>(stateQueue.ToArray());
				_state = _stateQueue.Dequeue();
			}
			else
			{
				_state = null;
			}
		}

		/// <summary>
		/// 
		/// </summary>
		public override bool Read()
		{
			if (_state != null && _stateQueue.Count > 0)
			{
				_state = _stateQueue.Dequeue();
				return true;
			}
			else
			{
				_state = null;
				var result = _reader.Read();
				return result;
			}
		}

	}

	/// <summary>
	/// 
	/// </summary>
	public class SerializeContractResolver : DefaultContractResolver
	{
		/// <summary>
		/// 
		/// </summary>
		public static readonly SerializeContractResolver Instance = new SerializeContractResolver();

		/// <summary>
		/// 
		/// </summary>
		/// <param name="type"></param>
		/// <param name="memberSerialization"></param>
		/// <returns></returns>
		protected override IList<JsonProperty> CreateProperties(Type type, MemberSerialization memberSerialization)
		{
			if (typeof(Exception).IsAssignableFrom(type) && type != typeof(object))
			{
				var propertyInfos = type.GetProperties(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);

				var ps = new List<JsonProperty>();
				foreach (var pi in propertyInfos)
				{
					var p = CreateProperty(pi, memberSerialization);
					ps.Add(p);
				}

				var member = typeof(Exception).GetField("_className", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
				if (member != null)
				{
					var p = CreateProperty(member, memberSerialization);
					//p.ValueProvider = new ValueProvider<string>(new FieldValueSource<Exception, string>(member));
					p.ValueProvider = new ValueProvider<string>(new GetterSetterFieldValueSource<Exception, string>(it => it.GetType().FullName, null));
					p.PropertyName = "ClassName";
					p.Readable = true;
					p.Writable = false;
					ps.Insert(0, p);
				}
				return ps;
			}

			var properties = base.CreateProperties(type, memberSerialization);
			return properties;
		}

		//protected override string ResolvePropertyName(string propertyName)
		//{
		//	return base.ResolvePropertyName(propertyName);
		//}

		//protected override JsonProperty CreatePropertyFromConstructorParameter(JsonProperty matchingMemberProperty, ParameterInfo parameterInfo)
		//{
		//	return base.CreatePropertyFromConstructorParameter(matchingMemberProperty, parameterInfo);
		//}

		//protected override IValueProvider CreateMemberValueProvider(MemberInfo member)
		//{
		//	return base.CreateMemberValueProvider(member);
		//}

		/// <summary>
		/// 
		/// </summary>
		protected override JsonProperty CreateProperty(MemberInfo member, MemberSerialization memberSerialization)
		{
			var property = base.CreateProperty(member, memberSerialization);

			if (member.DeclaringType == typeof(Exception))
			{
				string valueName;
				if (ExceptionFieldValueToNameDictionary.TryGetValue(member.Name, out valueName))
				{
					//property.ValueProvider = new ValueProvider(member.Name, valueName, member);
					var valueProvider = new ValueProvider(PropertyReflector.MakeObjectPropertyGetter(PropertyReflector.GetProperty(typeof(Exception), member.Name)),
						PropertyReflector.MakeObjectFieldSetter(PropertyReflector.GetField(typeof(Exception), valueName)))
					{
						Tag = string.Format("DeclaringType:{0}, Read: {1}, Write: {2}", member.DeclaringType.FullName, member.Name, valueName)
					};
					property.ValueProvider = valueProvider;
					property.Writable = true;
				}
			}

			return property;
		}

		//public class PropertyGetFieldSetValueSource<TValue> : IValueSource<TValue>
		//{
		//	private FieldInfo _field;
		//	private PropertyInfo _property;

		//	public PropertyGetFieldSetValueSource(PropertyInfo property, FieldInfo field)
		//	{
		//		_property = property;
		//		_field = field;
		//	}

		//	public TValue GetValue(object target)
		//	{
		//		return (TValue)_property.GetValue(target, new object[0]);
		//	}

		//	public void SetValue(object target, TValue value)
		//	{
		//		_field.SetValue(target, value);
		//	}
		//}

		/// <summary>
		/// 
		/// </summary>
		/// <typeparam name="TTarget"></typeparam>
		/// <typeparam name="TValue"></typeparam>
		public class FuncValueSource<TTarget, TValue> : IValueSource<TValue>
		{
			private readonly Func<TTarget, TValue> _getter;
			private readonly Action<TTarget, TValue> _setter;

			/// <summary>
			/// 
			/// </summary>
			/// <param name="getter"></param>
			/// <param name="setter"></param>
			public FuncValueSource(Func<TTarget, TValue> getter, Action<TTarget, TValue> setter)
			{
				_getter = getter;
				_setter = setter;
			}

			/// <summary>
			/// 
			/// </summary>
			/// <param name="target"></param>
			/// <returns></returns>
			public TValue GetValue(object target)
			{
				return _getter((TTarget)target);
			}

			/// <summary>
			/// 
			/// </summary>
			/// <param name="target"></param>
			/// <param name="value"></param>
			public void SetValue(object target, TValue value)
			{
				_setter((TTarget)target, value);
			}
		}

		interface IValueSource<TValue>
		{
			TValue GetValue(object target);

			void SetValue(object target, TValue value);
		}

		/// <summary>
		/// 
		/// </summary>
		public class GetterSetterFieldValueSource<TTarget, TValue> : IValueSource<TValue>
		{
			Func<TTarget, TValue> _getter;
			Action<TTarget, TValue> _setter;

			/// <summary>
			/// 
			/// </summary>
			/// <param name="getter"></param>
			/// <param name="setter"></param>
			public GetterSetterFieldValueSource(Func<TTarget, TValue> getter, Action<TTarget, TValue> setter)
			{
				_getter = getter;
				_setter = setter;
			}

			/// <summary>
			/// 
			/// </summary>
			/// <param name="target"></param>
			/// <returns></returns>
			public TValue GetValue(object target)
			{
				return _getter == null
					? default(TValue)
					: _getter((TTarget)target);
			}

			/// <summary>
			/// 
			/// </summary>
			/// <param name="target"></param>
			/// <param name="value"></param>
			public void SetValue(object target, TValue value)
			{
				if (_setter != null)
					_setter((TTarget)target, value);
			}
		}

		class ValueProvider<TValue> : IValueProvider
		{
			private readonly IValueSource<TValue> _source;

			public ValueProvider(IValueSource<TValue> source)
			{
				_source = source;
			}

			public void SetValue(object target, object value)
			{
				try
				{
					_source.SetValue(target, (TValue)value);
				}
				catch (Exception ex)
				{
					LogHelper.Debug(ex);
					throw;
				}
			}

			public object GetValue(object target)
			{
				//return _field.GetValue(target);
				return _source.GetValue(target);
			}
		}

		//		class PropertyValueProvider : IValueProvider
		//		{
		//			private string _name;
		//			private string _valueName;
		//			private PropertyInfo _property;
		//			private FieldInfo _field;

		//			public PropertyValueProvider(string name, string valueName, MemberInfo memberInfo)
		//			{
		//				_name = name;
		//				_valueName = valueName;

		//				_field = memberInfo.DeclaringType
		//#if NETCORE
		//					.GetTypeInfo()
		//#endif
		//					.GetField(valueName, BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public);
		//				if (_field == null)
		//				{
		//				}
		//				_property = (PropertyInfo)memberInfo;
		//			}

		//			public void SetValue(object target, object value)
		//			{
		//				try
		//				{
		//					_field.SetValue(target, value);
		//				}
		//				catch (Exception)
		//				{
		//					throw;
		//				}
		//			}

		//			public object GetValue(object target)
		//			{
		//				return _property.GetValue(target, new object[0]);
		//			}
		//		}

		/// <summary>
		/// 
		/// </summary>
		public class ValueProvider : IValueProvider
		{
			private readonly Func<object, object> _getter;
			private readonly Action<object, object> _setter;

			/// <summary>
			/// 
			/// </summary>
			/// <param name="getter"></param>
			/// <param name="setter"></param>
			public ValueProvider(Func<object, object> getter, Action<object, object> setter)
			{
				_getter = getter;
				_setter = setter;
			}

			/// <summary>
			/// 
			/// </summary>
			public object Tag { get; set; }

			/// <summary>
			/// 
			/// </summary>
			/// <param name="target"></param>
			/// <param name="value"></param>
			public void SetValue(object target, object value)
			{
				if (_setter == null)
					return;

				try
				{
					_setter(target, value);
				}
				catch (Exception ex)
				{
					LogHelper.Debug(ex);
					throw;
				}
			}

			/// <summary>
			/// 
			/// </summary>
			/// <param name="target"></param>
			/// <returns></returns>
			public object GetValue(object target)
			{
				return _getter?.Invoke(target);
			}
		}

		static readonly Dictionary<string, string> EssentialExceptionFields = new Dictionary<string, string>(12)
		{
			{"_className", "ClassName"},
			{"_message", "Message"},
			{"_data", "Data"},
			{"_innerException", "InnerException"},
			{"_helpURL", "HelpURL"},
			{"_stackTraceString", "StackTrace"},
			{"_remoteStackIndex", "RemoteStackIndex"},
			{"_exceptionMethodString", "ExceptionMethod"},
			{"_HResult", "HResult"},
			{"_source", "Source"},
			{"_watsonBuckets", "WatsonBuckets"}
		};

		static readonly Dictionary<string, string> ExceptionFieldValueToNameDictionary = EssentialExceptionFields.ToDictionary(it => it.Value, it => it.Key);

	}
}
