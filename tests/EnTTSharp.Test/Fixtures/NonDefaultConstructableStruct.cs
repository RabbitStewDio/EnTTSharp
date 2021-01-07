using EnTTSharp.Entities.Attributes;

namespace EnTTSharp.Test.Fixtures
{
    [EntityComponent(EntityConstructor.NonConstructable)]
    public struct NonDefaultConstructableStruct
    {
    }
}