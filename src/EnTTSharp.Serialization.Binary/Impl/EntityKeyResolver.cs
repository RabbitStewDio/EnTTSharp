using System;
using EnTTSharp.Entities;
using MessagePack;
using MessagePack.Formatters;

namespace EnTTSharp.Serialization.Binary.Impl
{
    public class EntityKeyResolver:IFormatterResolver
    {
        readonly Func<EntityKeyData, EntityKey> entityMapper;
        EntityKeyFormatter keyFormatterInstance;
        EntityKeyDataFormatter dataFormatterInstance;

        public IMessagePackFormatter<T> GetFormatter<T>()
        {
            if (typeof(T) == typeof(EntityKey))
            {
                if (keyFormatterInstance == null)
                {
                    keyFormatterInstance = new EntityKeyFormatter(entityMapper);
                }

                return (IMessagePackFormatter<T>)keyFormatterInstance;
            }

            if (typeof(T) == typeof(EntityKeyData))
            {
                if (dataFormatterInstance == null)
                {
                    dataFormatterInstance = new EntityKeyDataFormatter();
                }

                return (IMessagePackFormatter<T>) dataFormatterInstance;
            }

            return null;
        }

        public EntityKeyResolver(Func<EntityKeyData, EntityKey> mapper = null)
        {
            this.entityMapper = mapper ?? Map;
            Console.WriteLine("Entity Mapper is " + entityMapper);
        }

        EntityKey Map(EntityKeyData data)
        {
            Console.WriteLine("Standard mapper used");
            return new EntityKey(data.Age, data.Key);
        }

        class EntityKeyDataFormatter : IMessagePackFormatter<EntityKeyData>
        {
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

        class EntityKeyFormatter : IMessagePackFormatter<EntityKey>
        {
            readonly Func<EntityKeyData, EntityKey> entityMapper;

            public EntityKeyFormatter(Func<EntityKeyData, EntityKey> entityMapper)
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
                var r = entityMapper(new EntityKeyData(age, key));
                Console.WriteLine("Reading " + age + " " + key + " -> " + r);
                return r;

            }
        }
    }
}