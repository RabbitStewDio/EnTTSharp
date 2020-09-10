using EnTTSharp.Annotations;
using EnTTSharp.Entities.Attributes;

namespace EnTTSharp.Test.Annotations
{
    [EntityComponent(EntityConstructor.NonConstructable)]
    public class ProhibitDefaultConstrucableClassComponent
    {
    }
}