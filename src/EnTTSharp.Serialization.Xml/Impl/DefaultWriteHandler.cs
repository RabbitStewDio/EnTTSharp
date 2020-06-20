﻿using System;
using System.Xml;
using System.Xml.Serialization;

namespace EnTTSharp.Serialization.Xml
{
    public class DefaultWriteHandler<T>
    {
        readonly XmlSerializer serializer;

        public DefaultWriteHandler() : this(new XmlSerializer(typeof(T)))
        {
        }

        public DefaultWriteHandler(XmlSerializer serializer)
        {
            this.serializer = serializer ?? throw new ArgumentNullException(nameof(serializer));
        }

        public void Write(XmlWriter writer, T component)
        {
            serializer.Serialize(writer, component);
        }
    }
}