using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Serialization;

namespace ServiceTest.ClientTest
{
	public class JsonSerializerTester
	{
		public static void JsonSerializerTest()
		{
			object exobj = null;
			try
			{
				try
				{
					var b = 3;
					b = 0;
					var c = 2 / b;
				}
				catch (Exception ex)
				{
					throw new InvalidOperationException("test ops ex", ex);
				}
			}
			catch (Exception ex)
			{
				exobj = ex;
			}

			try
			{
				throw new Exception("test ex");
			}
			catch (Exception ex)
			{
				//exobj = ex;
			}

			//var exToSerialize = new Exception("test ex");
			var exToSerialize = exobj;
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

			//var exts1 = new ArgumentNullException();

			{
				try
				{
					var setting = new JsonSerializerSettings
					{
						Formatting = Formatting.Indented,
						ContractResolver = new SerializeContractResolver(),
						Converters = new List<JsonConverter>
						{
							new ExceptionConverter(),
							//new EmptyConverter(),
							new KeyValuePairConverter(),
							new ExpandoObjectConverter()
						}
					};
					var json = JsonConvert.SerializeObject(exToSerialize, setting);
					Console.WriteLine(json);
					//var dex2 = JsonConvert.DeserializeObject(json, exToSerialize.GetType(), new EmptyConverter());
					var dex = JsonConvert.DeserializeObject(json, exToSerialize.GetType(), setting);
				}
				catch (Exception ex)
				{
					throw;
				}
			}
		}
	}

	class ReflectTest
	{

		//public delegate TValue MemberGetDelegate<TTarget, TValue>(TTarget obj);

		public static void Test()
		{
			var type = typeof(Exception);
			var field = type.GetField("_className", BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public);
			var dm = new DynamicMethod("Get" + field.Name, typeof(string), new[] { type }, type);
			var il = dm.GetILGenerator();
			// Load the instance of the object (argument 0) onto the stack
			il.Emit(OpCodes.Ldarg_0);
			// Load the value of the object's field (fi) onto the stack
			il.Emit(OpCodes.Ldfld, field);
			// return the value on the top of the stack
			il.Emit(OpCodes.Ret);
			var getter = dm.CreateDelegate(typeof(Func<Exception, string>)) as Func<Exception, string>;
			if (getter != null)
			{
				{
					var exobj = new Exception("test ex");
					//var str = exobj.ToString();
					var className = getter(exobj);
				}

			}
		}
	}

	public class EmptyConverter : CustomCreationConverter<Exception>
	{
		public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
		{
			var obj = base.ReadJson(reader, objectType, existingValue, serializer);
			return obj;
		}

		public override Exception Create(Type objectType)
		{
			return (Exception)Activator.CreateInstance(objectType);
		}
	}

	public class ExceptionConverter : CustomCreationConverter<Exception>
	{
		private string _className;

		public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
		{
			if (reader.TokenType == JsonToken.Null)
			{
				return base.ReadJson(reader, objectType, existingValue, serializer);
			}

			var stateQueue = new Queue<ReaderState>(2);

			var state0 = GetState(reader);
			stateQueue.Enqueue(state0);

			reader.Read();
			var state1 = GetState(reader);
			stateQueue.Enqueue(state1);

			if (state1.ValueType == typeof(string) && (string)state1.Value == "_className")
			{
				reader.Read();
				var state2 = GetState(reader);
				stateQueue.Enqueue(state2);
				_className = (string)reader.Value;
			}

			var newReader = new BufferJsonReader(reader, stateQueue);

			try
			{
				var obj = base.ReadJson(newReader, objectType, existingValue, serializer);
				return obj;
			}
			catch (Exception ex)
			{
				throw;
			}

		}

		private static ReaderState GetState(JsonReader reader)
		{
			return new ReaderState
			{
				TokenType = reader.TokenType,
				ValueType = reader.ValueType,
				Value = reader.Value,
				Path = reader.Path,
				Depth = reader.Depth,
				QuoteChar = reader.QuoteChar,
			};
		}

		public override Exception Create(Type objectType)
		{
			var type = (objectType.FullName != _className)
				? Type.GetType(_className)
				: objectType;

			//var type = objectType;

			return (Exception)Activator.CreateInstance(type);
		}
	}

	public class ReaderState
	{
		public JsonToken TokenType { get; internal set; }
		public Type ValueType { get; internal set; }
		public object Value { get; internal set; }
		public string Path { get; internal set; }
		public int Depth { get; internal set; }
		public char QuoteChar { get; internal set; }
	}

	public class BufferJsonReader : JsonReader
	{
		private Queue<ReaderState> _stateQueue;
		private JsonReader _reader;
		private ReaderState _state;

		public BufferJsonReader(JsonReader reader)
		{
		}

		public override JsonToken TokenType
		{
			get
			{
				return _state != null ? _state.TokenType : _reader.TokenType;
			}
		}

		public override Type ValueType { get { return _state != null ? _state.ValueType : _reader.ValueType; } }

		public override object Value { get { return _state != null ? _state.Value : _reader.Value; } }

		public override string Path { get { return _state != null ? _state.Path : _reader.Path; } }

		public override int Depth { get { return _state != null ? _state.Depth : _reader.Depth; } }

		public override char QuoteChar { get { return _state != null ? _state.QuoteChar : _reader.QuoteChar; } }

		public override void Close()
		{
			_reader.Close();
		}

		public override string ReadAsString()
		{
			var value = base.ReadAsString();
			return value;
			//return base.ReadAsString();
		}

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

		public BufferJsonReader(JsonReader reader, Queue<ReaderState> stateQueue) : this(reader)
		{
			_reader = reader;
			if (stateQueue != null && stateQueue.Count > 0)
			{
				_stateQueue = new Queue<ReaderState>(stateQueue.ToArray());
				_state = _stateQueue.Dequeue();
			}
			else
			{
				_state = null;
			}
		}

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

	public class SerializeContractResolver : DefaultContractResolver
	{
		public static readonly SerializeContractResolver Instance = new SerializeContractResolver();

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
					p.PropertyName = "_className";
					p.Readable = true;
					p.Writable = false;
					ps.Insert(0, p);

					//var property = new JsonProperty()
					//{
					//	ValueProvider = new ValueProvider<string>(new FieldValueSource<string>(member)),
					//	PropertyName = "_className",
					//	Readable = true,
					//	Writable = true,
					//};

					//ps.Add(property);
				}
				return ps;
			}

			var properties = base.CreateProperties(type, memberSerialization);
			return properties;
		}

		protected override string ResolvePropertyName(string propertyName)
		{
			return base.ResolvePropertyName(propertyName);
		}

		protected override JsonProperty CreatePropertyFromConstructorParameter(JsonProperty matchingMemberProperty, ParameterInfo parameterInfo)
		{
			return base.CreatePropertyFromConstructorParameter(matchingMemberProperty, parameterInfo);
		}

		protected override IValueProvider CreateMemberValueProvider(MemberInfo member)
		{
			return base.CreateMemberValueProvider(member);
		}

		protected override JsonProperty CreateProperty(MemberInfo member, MemberSerialization memberSerialization)
		{
			var property = base.CreateProperty(member, memberSerialization);

			if (member.DeclaringType == typeof(Exception))
			{
				string valueName;
				if (_essentialExceptionFields2.TryGetValue(member.Name, out valueName))
				{
					property.ValueProvider = new ValueProvider(member.Name, valueName, member);
					property.Writable = true;
				}
			}
			//else
			//	property.ValueProvider = new ValueProvider(member.Name, member.Name, member);

			return property;
		}

		interface IValueSource<TValue>
		{
			TValue GetValue(object target);

			void SetValue(object target, TValue value);
		}

		public class PropertyGetFieldSetValueSource<TValue> : IValueSource<TValue>
		{
			private FieldInfo _field;
			private PropertyInfo _property;

			public PropertyGetFieldSetValueSource(PropertyInfo property, FieldInfo field)
			{
				_property = property;
				_field = field;
			}

			public TValue GetValue(object target)
			{
				return (TValue)_property.GetValue(target);
			}

			public void SetValue(object target, TValue value)
			{
				_field.SetValue(target, value);
			}
		}

		public class FieldValueSource<TTarget, TValue> : IValueSource<TValue>
		{
			private FieldInfo _field;
			public delegate TValue MemberGetDelegate(TTarget obj);

			MemberGetDelegate _getter;

			public FieldValueSource(FieldInfo field)
			{
				_field = field;

				var dm = new DynamicMethod("Get" + field.Name, typeof(TValue), new Type[] { typeof(TTarget) }, typeof(TTarget));
				var il = dm.GetILGenerator();
				// Load the instance of the object (argument 0) onto the stack
				il.Emit(OpCodes.Ldarg_0);
				// Load the value of the object's field (fi) onto the stack
				il.Emit(OpCodes.Ldfld, field);
				// return the value on the top of the stack
				il.Emit(OpCodes.Ret);

				_getter = (MemberGetDelegate)dm.CreateDelegate(typeof(MemberGetDelegate));
			}

			public TValue GetValue(object target)
			{
				//var value = target.GetType() == typeof(TTarget)
				//	? (TValue)_field.GetValue(target)
				//	: (TValue)_field.GetValue((TTarget)target);

				//value = (TValue)_field.GetValue((TTarget)target);

				var value = _getter((TTarget)target);
				return value;
			}

			public void SetValue(object target, TValue value)
			{
				if (target.GetType() == typeof(TTarget))
					_field.SetValue(target, value);
				else
					_field.SetValue((TTarget)target, value);
				//_field.SetValue((TTarget)target, value);
			}
		}

		public class FieldValueSource<TValue> : IValueSource<TValue>
		{
			private FieldInfo _field;

			public FieldValueSource(FieldInfo field)
			{
				_field = field;
			}

			public TValue GetValue(object target)
			{
				return (TValue)_field.GetValue(target);
			}

			public void SetValue(object target, TValue value)
			{
				_field.SetValue(target, value);
			}
		}

		public class GetterSetterFieldValueSource<TTarget, TValue> : IValueSource<TValue>
		{
			Func<TTarget, TValue> _getter;
			Action<TTarget, TValue> _setter;

			public GetterSetterFieldValueSource(Func<TTarget, TValue> getter, Action<TTarget, TValue> setter)
			{
				_getter = getter;
				_setter = setter;
			}

			public TValue GetValue(object target)
			{
				return _getter == null
					? default(TValue)
					: _getter((TTarget)target);
			}

			public void SetValue(object target, TValue value)
			{
				if (_setter != null)
					_setter((TTarget)target, value);
			}
		}

		public class PropertyValueSource<TValue> : IValueSource<TValue>
		{
			private PropertyInfo _property;

			public PropertyValueSource(PropertyInfo property)
			{
				_property = property;
			}

			public TValue GetValue(object target)
			{
				return (TValue)_property.GetValue(target);
			}

			public void SetValue(object target, TValue value)
			{
				_property.SetValue(target, value);
			}
		}

		class ValueProvider<TValue> : IValueProvider
		{
			private IValueSource<TValue> _source;
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
				catch (Exception)
				{
					throw;
				}
			}

			public object GetValue(object target)
			{
				//return _field.GetValue(target);
				return _source.GetValue(target);
			}
		}

		class ValueProvider : IValueProvider
		{
			private string _name;
			private string _valueName;
			private PropertyInfo _property;
			private FieldInfo _field;

			public ValueProvider(string name, string valueName, MemberInfo memberInfo)
			{
				_name = name;
				_valueName = valueName;

				_field = memberInfo.DeclaringType.GetTypeInfo().GetField(valueName, BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public);
				if (_field == null)
				{
				}
				_property = (PropertyInfo)memberInfo;
			}

			public void SetValue(object target, object value)
			{
				try
				{
					_field.SetValue(target, value);
				}
				catch (Exception)
				{
					throw;
				}
			}

			public object GetValue(object target)
			{
				//return _field.GetValue(target);
				return _property.GetValue(target);
			}
		}

		static Dictionary<string, string> _essentialExceptionFields = new Dictionary<string, string>(12)
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

		static Dictionary<string, string> _essentialExceptionFields2 = _essentialExceptionFields.ToDictionary(it => it.Value, it => it.Key);

		private static Dictionary<string, string> CreateExceptionFields()
		{
			var essentialExceptionFields = new Dictionary<string, string>(12)
			{
				{"_className", "ClassName"},
				{"_message", "Message"},
				{"_data", "Data"},
				{"_innerException", "InnerException"},
				{"_helpURL", "HelpURL"},
				{"_stackTraceString", "StackTraceString"},
				{"_remoteStackTraceString", "RemoteStackTraceString"},
				{"_remoteStackIndex", "RemoteStackIndex"},
				{"_exceptionMethodString", "ExceptionMethod"},
				{"_HResult", "HResult"},
				{"_source", "Source"},
				{"_watsonBuckets", "WatsonBuckets"}
			};
			return essentialExceptionFields;
		}
	}

}
