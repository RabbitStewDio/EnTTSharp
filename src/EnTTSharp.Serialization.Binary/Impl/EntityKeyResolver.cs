using EnttSharp.Entities;
using EnTTSharp.Entities;
using MessagePack;
using MessagePack.Formatters;

namespace EnTTSharp.Serialization.Binary.Impl
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
            }

            public EntityKey Deserialize(ref MessagePackReader reader, MessagePackSerializerOptions options)
            {
                var age = reader.ReadByte();
                var key = reader.ReadInt32();
                return new EntityKey(age, key);
            }
        }
    }
}