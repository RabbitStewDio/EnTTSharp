using EnTTSharp.Annotations;

namespace EnTTSharp.Test.Annotations
{
    [EntityComponent(EntityConstructor.NonConstructable)]
    public struct NonDefaultConstructableStruct
    {
    }
}