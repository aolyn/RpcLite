#define USEPROTOBUF

using System;
using System.Collections.Generic;
using System.IO;
using ProtoBuf;
using ProtoBuf.Meta;

#if USEPROTOBUF
#endif

namespace RpcLite.Formatters.Protobuf
{
    internal class ProtobufHelper
    {
        private static readonly RuntimeTypeModel TypeModel;

        static ProtobufHelper()
        {
#if USEPROTOBUF
            TypeModel = ProtoBuf.Meta.TypeModel.Create();
            //TypeModel[typeof(KeyValuePair<string, object>)].SetSurrogate(typeof(TagRefPair));
#endif
        }

        [ProtoContract]
        public class TagRefPair
        {
            //private static readonly IDictionary<string, Type> TypeObjectMapping;
            private static readonly IDictionary<Type, string> TypeNameMapping;

            [ProtoMember(1)]
            public string Key { get; set; }

            [ProtoMember(2)]
            public string Type { get; set; }

            [ProtoMember(3)]
            public string Value { get; set; }

            static TagRefPair()
            {
                TypeNameMapping = new Dictionary<Type, string>
                {
                    [typeof(string)] = "string",
                    [typeof(int)] = "int32",
                    [typeof(int)] = "int64",
                    [typeof(byte)] = "byte",
                };

                //TypeObjectMapping = new Dictionary<string, Type>();
                //foreach (var item in TypeNameMapping)
                //{
                //    TypeObjectMapping[item.Value] = item.Key;
                //}
            }

            public TagRefPair()
            {
            }

            public TagRefPair(string key, object value) : this()
            {
                Key = key;

                if (value == null)
                {
                    Type = "object";
                }
                else
                {
                    var type = value.GetType();
                    Value = value.ToString();

                    if (!TypeNameMapping.TryGetValue(type, out var typeName))
                    {
                        typeName = "string";
                    }
                    Type = typeName;
                }
            }

            public object GetValue()
            {
                if (Value == null) return null;

                //TypeObjectMapping.TryGetValue(Type, out var type);
                switch (Type)
                {
                    case "string":
                        return Value;
                    case "int32":
                        return int.Parse(Value);
                    case "int64":
                        return long.Parse(Value);
                    case "byte":
                        return byte.Parse(Value);
                    default:
                        return Value;
                }
            }

            public static implicit operator KeyValuePair<string, object>(TagRefPair val)
            {
                return new KeyValuePair<string, object>(val.Key, val.GetValue());
            }

            public static implicit operator TagRefPair(KeyValuePair<string, object> val)
            {
                return new TagRefPair(val.Key, val.Value);
            }
        }

        //[ProtoContract]
        //private class ObjectSurrogate
        //{
        //    [ProtoMember(1)]
        //    public string Value { get; set; }
        //    //public static implicit operator ObjectSurrogate(object obj)
        //    //{
        //    //    return new ObjectSurrogate
        //    //    {
        //    //        Value = obj?.ToString()
        //    //    };
        //    //}
        //    public static implicit operator object(ObjectSurrogate obj)
        //    {
        //        return new ObjectSurrogate
        //        {
        //            Value = obj?.ToString()
        //        };
        //    }
        //}

        public static byte[] Serialize(object obj)
        {
#if USEPROTOBUF
            if (obj == null) return new byte[0];

            using (var ms = new MemoryStream())
            {
                Serialize(obj, ms);
                return ms.ToArray();
            }
#else
			return new byte[0];
#endif
        }

        public static void Serialize(object obj, Stream ms)
        {
            TypeModel.Serialize(ms, obj);
        }

        //public static void Serialize(object obj, BinaryWriter writer)
        //{
        //	using (var ms = new BinaryWriterStream(writer))
        //	{
        //		Serializer.Serialize(ms, obj);
        //	}
        //}

        public static object Deserialize(Type type, Stream source)
        {
#if USEPROTOBUF
            return TypeModel.Deserialize(source, null, type);
#else
			return null;
#endif
        }

        public static object Deserialize<T>(Stream source)
        {
#if USEPROTOBUF
            return Serializer.Deserialize(typeof(T), source);
#else
			return default(T);
#endif
        }
    }


}
