using EnTTSharp.Entities;

namespace EnTTSharp.Serialization.Xml
{
    public class EntityKeySurrogateProvider: SerializationSurrogateProviderBase<EntityKey, EntityKeyData>
    {
        readonly IEntityKeyMapper entityMapper;

        public EntityKeySurrogateProvider(IEntityKeyMapper entityMapper = null)
        {
            this.entityMapper = entityMapper ?? new DefaultEntityKeyMapper().Register(Map);
        }

        EntityKey Map(EntityKeyData surrogate)
        {
            return new EntityKey(surrogate.Age, surrogate.Key);
        }

        public override EntityKey GetDeserializedObject(EntityKeyData surrogate)
        {
            return entityMapper.EntityKeyMapper<EntityKey>(surrogate);
        }

        public override EntityKeyData GetObjectToSerialize(EntityKey obj)
        {
            return new EntityKeyData(obj.Age, obj.Key);
        }
    }
}