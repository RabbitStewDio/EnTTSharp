using System;

namespace EnTTSharp.Entities.Attributes
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct | AttributeTargets.Enum | AttributeTargets.Interface)]
    public class EntityComponentAttribute: Attribute
    {
        public EntityComponentAttribute(EntityConstructor constructable = EntityConstructor.Auto)
        {
            Constructor = constructable;
        }

        public EntityConstructor Constructor { get; set; } 
    }
}
