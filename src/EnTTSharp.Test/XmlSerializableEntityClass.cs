using System;
using System.Xml;
using EnttSharp.Entities;

namespace EnTTSharp.Test
{
    public class XmlSerializableEntityClass
    {
        public static XmlSerializableEntityClass ReadFromXml(XmlReader reader, Func<EntityKey, EntityKey> translator)
        {
            return new XmlSerializableEntityClass();
        }
    }
}