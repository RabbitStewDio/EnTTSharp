using System;
using EnttSharp.Entities;

namespace EnTTSharp.Serialization
{
    public interface IEntityArchiveReader
    {
        int ReadEntityCount();
        EntityKey ReadEntity(Func<EntityKey, EntityKey> entityMapper);

        int ReadComponentCount<TComponent>();
        bool TryReadComponent<TComponent>(Func<EntityKey, EntityKey> entityMapper, out EntityKey key, out TComponent component);

        bool ReadTagFlag<TComponent>();
        bool TryReadTag<TComponent>(Func<EntityKey, EntityKey> entityMapper, out EntityKey entityKey, out TComponent component);

        int ReadDestroyedCount();
        EntityKey ReadDestroyed(Func<EntityKey, EntityKey> entityMapper);

    }
}