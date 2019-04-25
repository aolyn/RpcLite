using System;
using System.Collections.Generic;
using System.IO;

namespace RpcLite.Formatters.Protobuf
{
    public class ProtobufFormatter : IFormatter
    {
        public object Deserialize(Stream inputStream, Type targetType)
        {
            return ProtobufHelper.Deserialize(targetType, inputStream);
        }

        public object Deserialize(TextReader reader, Type targetType)
        {
            throw new NotSupportedException();
        }

        public void Serialize(Stream outputStream, object source, Type type)
        {
            ProtobufHelper.Serialize(source, outputStream);
        }

        public void Serialize(TextWriter writer, object source, Type type)
        {
            throw new NotSupportedException();
        }

        public string Name { get; } = "protobuf";

        public List<string> SupportMimes { get; } = new List<string>
        {
            "application/protobuf"
        };

        public bool SupportException { get; } = false;
    }
}
