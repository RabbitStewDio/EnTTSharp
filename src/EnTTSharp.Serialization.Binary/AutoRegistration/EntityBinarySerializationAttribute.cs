using System;

namespace EnTTSharp.Serialization.BinaryPack.AutoRegistration
{
    [AttributeUsage(AttributeTargets.Class| AttributeTargets.Enum | AttributeTargets.Struct)]
    public sealed class EntityBinarySerializationAttribute : Attribute
    {
        public string ComponentTypeId { get; set; }
        public bool UsedAsTag { get; set; }
    }
}