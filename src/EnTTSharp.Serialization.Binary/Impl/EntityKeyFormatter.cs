using EnTTSharp.Entities;
using MessagePack;
using MessagePack.Formatters;

namespace EnTTSharp.Serialization.Binary.Impl
{
    public class EntityKeyFormatter : IMessagePackFormatter<EntityKey>
    {
        readonly IEntityKeyMapper entityMapper;

        public EntityKeyFormatter(IEntityKeyMapper entityMapper)
        {
            this.entityMapper = entityMapper;
        }

        public void Serialize(ref MessagePackWriter writer, EntityKey value, MessagePackSerializerOptions options)
        {
            writer.Write(value.Age);
            writer.Write(value.Key);
        }

        public EntityKey Deserialize(ref MessagePackReader reader, MessagePackSerializerOptions options)
        {
            var age = reader.ReadByte();
            var key = reader.ReadInt32();
            var r = entityMapper.EntityKeyMapper<EntityKey>(new EntityKeyData(age, key));
            return r;
        }
    }
}