using EnttSharp.Entities;
using MessagePack;
using MessagePack.Formatters;

namespace EnTTSharp.Serialization.BinaryPack
{
    public class EntityKeyResolver:IFormatterResolver
    {
        public IMessagePackFormatter<T> GetFormatter<T>()
        {
            if (typeof(T) == typeof(EntityKey))
            {
                return (IMessagePackFormatter<T>) FormatterInstance;
            }

            return null;
        }

        static readonly EntityKeyFormatter FormatterInstance = new EntityKeyFormatter();

        class EntityKeyFormatter : IMessagePackFormatter<EntityKey>
        {
            public void Serialize(ref MessagePackWriter writer, EntityKey value, MessagePackSerializerOptions options)
            {
                writer.Write(value.Age);
                writer.Write(value.Key);
                writer.Write(value.Extra);
            }

            public EntityKey Deserialize(ref MessagePackReader reader, MessagePackSerializerOptions options)
            {
                var age = reader.ReadByte();
                var key = reader.ReadInt32();
                var extra = reader.ReadUInt32();
                return new EntityKey(age, key, extra);
            }
        }
    }
}