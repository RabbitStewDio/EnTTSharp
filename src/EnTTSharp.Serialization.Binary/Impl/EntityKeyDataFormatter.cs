using MessagePack;
using MessagePack.Formatters;

namespace EnTTSharp.Serialization.Binary.Impl
{
    public class EntityKeyDataFormatter : IMessagePackFormatter<EntityKeyData>
    {
        public static readonly EntityKeyDataFormatter Instance = new EntityKeyDataFormatter();

        public void Serialize(ref MessagePackWriter writer, EntityKeyData value, MessagePackSerializerOptions options)
        {
            writer.Write(value.Age);
            writer.Write(value.Key);
        }

        public EntityKeyData Deserialize(ref MessagePackReader reader, MessagePackSerializerOptions options)
        {
            var age = reader.ReadByte();
            var key = reader.ReadInt32();
            return new EntityKeyData(age, key);
        }
    }
}