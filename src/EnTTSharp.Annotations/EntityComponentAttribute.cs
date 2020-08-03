using System;

namespace EnTTSharp.Annotations
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

    [AttributeUsage(AttributeTargets.Method)]
    public sealed class EntityDestructorAttribute : Attribute
    {
    }

}
