using System;
using System.Runtime.Serialization;
using System.Xml;

namespace EnTTSharp.Serialization.Xml
{
    public class DefaultDataContractWriteHandler<T>
    {
        readonly DataContractSerializer serializer;

        public DefaultDataContractWriteHandler() : this(new DataContractSerializer(typeof(T)))
        {
        }

        public DefaultDataContractWriteHandler(DataContractSerializer serializer)
        {
            this.serializer = serializer ?? throw new ArgumentNullException(nameof(serializer));
        }

        public void Write(XmlWriter writer, T component)
        {
            serializer.WriteObject(writer, component);
        }
    }
}