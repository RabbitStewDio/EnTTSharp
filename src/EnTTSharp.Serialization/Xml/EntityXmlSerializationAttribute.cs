using System;

namespace EnTTSharp.Serialization.Xml
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct)]
    public sealed class EntityXmlSerializationAttribute : Attribute
    {
        public string ComponentTypeId { get; set; }
    }
}