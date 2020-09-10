using System;

namespace EnTTSharp.Entities.Attributes
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct | AttributeTargets.Enum | AttributeTargets.Interface)]
    public class EntityKeyAttribute: Attribute
    {
    }
}