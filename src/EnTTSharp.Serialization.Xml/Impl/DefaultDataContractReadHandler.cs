using System;
using System.Runtime.Serialization;
using System.Xml;
using EnttSharp.Entities;

namespace EnTTSharp.Serialization.Xml
{
    public class DefaultDataContractReadHandler<TComponent>
    {
        readonly DataContractSerializer serializer;

        public DefaultDataContractReadHandler() : this(new DataContractSerializer(typeof(TComponent)))
        {
        }

        public DefaultDataContractReadHandler(DataContractSerializer serializer)
        {
            this.serializer = serializer ?? throw new ArgumentNullException(nameof(serializer));
        }

        public TComponent Read(XmlReader reader)
        {
            return (TComponent)serializer.ReadObject(reader);
        }
    }
}