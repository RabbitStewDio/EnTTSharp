using System.Runtime.Serialization;
using EnTTSharp.Annotations;
using EnTTSharp.Entities;
using EnTTSharp.Entities.Attributes;

namespace EnTTSharp.Test.Serialisation.NestedKeys
{
    /// <summary>
    ///   A test object that simulates a hierarchy or any other structure where
    ///   components reference other entities.
    /// </summary>
    [DataContract]
    [EntityComponent(EntityConstructor.NonConstructable)]
    public class NestedKeyComponent
    {
        [DataMember(Name = "ParentReference", Order = 0)]
        public EntityKey ParentReference { get; set; }

        public NestedKeyComponent(EntityKey parentReference)
        {
            ParentReference = parentReference;
        }
    }
}