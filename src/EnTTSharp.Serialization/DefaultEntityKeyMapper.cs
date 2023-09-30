using System;
using System.Collections.Generic;

namespace EnTTSharp.Serialization
{
    public class DefaultEntityKeyMapper: IEntityKeyMapper
    {
        readonly Dictionary<Type, object> data;

        public DefaultEntityKeyMapper()
        {
            data = new Dictionary<Type, object>();
        }

        public DefaultEntityKeyMapper Register<TEntityId>(Func<EntityKeyData, TEntityId> converter)
        {
            data[typeof(TEntityId)] = converter;
            return this;
        }
        
        public TEntityKey EntityKeyMapper<TEntityKey>(EntityKeyData data)
        {
            if (this.data.TryGetValue(typeof(TEntityKey), out var converter) &&
                converter is Func<EntityKeyData, TEntityKey> fn)
            {
                return fn(data);
            }
            
            throw new SnapshotIOException();
        }
    }
}