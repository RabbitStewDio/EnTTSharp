﻿using System;
using EnTTSharp.Entities;

namespace EnTTSharp.Serialization.Xml
{
    public class EntityKeySurrogateProvider: SerializationSurrogateProviderBase<EntityKey, EntityKeyData>
    {
        readonly Func<EntityKeyData, EntityKey> entityMapper;

        public EntityKeySurrogateProvider(Func<EntityKeyData, EntityKey> entityMapper = null)
        {
            this.entityMapper = entityMapper ?? Map;
        }

        EntityKey Map(EntityKeyData surrogate)
        {
            return new EntityKey(surrogate.Age, surrogate.Key);
        }

        public override EntityKey GetDeserializedObject(EntityKeyData surrogate)
        {
            return entityMapper(surrogate);
        }

        public override EntityKeyData GetObjectToSerialize(EntityKey obj)
        {
            return new EntityKeyData(obj.Age, obj.Key);
        }
    }
}