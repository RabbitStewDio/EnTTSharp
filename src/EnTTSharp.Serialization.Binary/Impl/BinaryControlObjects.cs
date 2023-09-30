using MessagePack;

namespace EnTTSharp.Serialization.Binary.Impl
{
    public static class BinaryControlObjects
    {
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
            [Key(0)]
            public readonly int ComponentCount;
            [Key(1)]
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
            [Key(0)]
            public readonly bool ComponentExists;
            [Key(1)]
            public readonly string ComponentId;

            public StartTagRecord(bool componentExists, string componentId)
            {
                ComponentExists = componentExists;
                ComponentId = componentId;
            }
        }
    }
}