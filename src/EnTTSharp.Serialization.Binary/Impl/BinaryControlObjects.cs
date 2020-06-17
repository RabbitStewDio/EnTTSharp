using MessagePack;
using MessagePack.Resolvers;

namespace EnTTSharp.Serialization.BinaryPack
{
    public static class BinaryControlObjects
    {
        public static MessagePackSerializerOptions CreateOptions(MessagePackSerializerOptions optionsRaw)
        {
            MessagePackSerializerOptions options;
            if (optionsRaw == null)
            {
                options = MessagePackSerializer.DefaultOptions.WithResolver(
                    CompositeResolver.Create(
                        new EntityKeyResolver(),
                        MessagePackSerializer.DefaultOptions.Resolver
                    ));
            }
            else
            {
                options = optionsRaw.WithResolver(
                    CompositeResolver.Create(
                        new EntityKeyResolver(),
                        optionsRaw.Resolver
                    ));
            }

            return options;
        }

        public enum BinaryStreamState
        {
            DestroyedEntities = 0,
            Entities = 1,
            Component = 2,
            Tag = 3,
            EndOfFrame = 4
        }

        [MessagePackObject]
        public readonly struct StartComponentRecord
        {
            public readonly int ComponentCount;
            public readonly string ComponentId;

            public StartComponentRecord(int componentCount, string componentId)
            {
                ComponentCount = componentCount;
                ComponentId = componentId;
            }
        }

        [MessagePackObject]
        public readonly struct StartTagRecord
        {
            public readonly bool ComponentExists;
            public readonly string ComponentId;

            public StartTagRecord(bool componentExists, string componentId)
            {
                ComponentExists = componentExists;
                ComponentId = componentId;
            }
        }
    }
}