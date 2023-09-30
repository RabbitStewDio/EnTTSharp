using EnTTSharp.Entities;
using MessagePack;
using MessagePack.Formatters;

namespace EnTTSharp.Serialization.Binary.Impl
{
    public class EntityKeyResolver : IFormatterResolver
    {
        readonly IEntityKeyMapper entityMapper;
        EntityKeyFormatter? keyFormatterInstance;

        public IMessagePackFormatter<T>? GetFormatter<T>()
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

        public EntityKeyResolver(IEntityKeyMapper? mapper = null)
        {
            this.entityMapper = mapper ?? new DefaultEntityKeyMapper().Register(Map);
        }
        
        EntityKey Map(EntityKeyData data)
        {
            return new EntityKey(data.Age, data.Key);
        }
        
        
    }
}