using System;
using EnTTSharp.Entities;
using MessagePack;
using MessagePack.Formatters;

namespace EnTTSharp.Serialization.Binary.Impl
{
    public class EntityKeyResolver:IFormatterResolver
    {
        readonly EntityKeyMapper<EntityKey> entityMapper;
        EntityKeyFormatter keyFormatterInstance;

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

            return null;
        }

        public EntityKeyResolver(EntityKeyMapper<EntityKey> mapper = null)
        {
            this.entityMapper = mapper ?? Map;
            Console.WriteLine("Entity Mapper is " + entityMapper);
        }

        EntityKey Map(EntityKeyData data)
        {
            Console.WriteLine("Standard mapper used");
            return new EntityKey(data.Age, data.Key);
        }
    }
}