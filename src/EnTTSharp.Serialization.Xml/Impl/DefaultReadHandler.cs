using System;
using System.Xml;
using System.Xml.Serialization;
using EnttSharp.Entities;

namespace EnTTSharp.Serialization.Xml
{
    public class DefaultReadHandler<TComponent>
    {
        readonly XmlSerializer serializer;

        public DefaultReadHandler() : this(new XmlSerializer(typeof(TComponent)))
        {
        }

        public DefaultReadHandler(XmlSerializer serializer)
        {
            this.serializer = serializer ?? throw new ArgumentNullException(nameof(serializer));
        }

        public TComponent Read(XmlReader reader)
        {
            return (TComponent)serializer.Deserialize(reader);
        }
    }
}