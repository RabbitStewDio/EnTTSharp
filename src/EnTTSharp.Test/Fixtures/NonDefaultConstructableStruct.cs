using EnTTSharp.Entities.Attributes;

namespace EnTTSharp.Test.Annotations
{
    [EntityComponent(EntityConstructor.NonConstructable)]
    public struct NonDefaultConstructableStruct
    {
    }
}