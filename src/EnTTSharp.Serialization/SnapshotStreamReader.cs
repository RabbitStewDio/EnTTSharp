using System;
using EnttSharp.Entities;

namespace EnTTSharp.Serialization
{
    /// <summary>
    ///   An ordered stream loader.
    /// </summary>
    public class SnapshotStreamReader
    {
        readonly ISnapshotLoader loader;
        readonly Func<EntityKey, EntityKey> entityMapper;

        public SnapshotStreamReader(ISnapshotLoader loader)
        {
            this.loader = loader;
            this.entityMapper = loader.Map;
        }

        public SnapshotStreamReader ReadEntities(IEntityArchiveReader reader)
        {
            var count = reader.ReadEntityCount();
            for (int c = 0; c < count; c += 1)
            {
                var entity = reader.ReadEntity(entityMapper);
                loader.OnEntity(entity);
            }

            return this;
        }

        public SnapshotStreamReader ReadComponent<TComponent>(IEntityArchiveReader reader)
        {
            var count = reader.ReadComponentCount<TComponent>();
            for (int c = 0; c < count; c += 1)
            {
                if (reader.TryReadComponent(entityMapper, out EntityKey entity, out TComponent component))
                {
                    loader.OnComponent(entity, component);
                }
            }

            return this;
        }

        public SnapshotStreamReader ReadTag<TComponent>(IEntityArchiveReader reader)
        {
            if (reader.ReadTagFlag<TComponent>())
            {
                if (reader.TryReadTag(entityMapper, out EntityKey entity, out TComponent component))
                {
                    loader.OnTag(entity, component);
                }
            }
            else
            {
                loader.OnTagRemoved<TComponent>();
            }

            return this;
        }

        public SnapshotStreamReader ReadDestroyed(IEntityArchiveReader reader)
        {
            var count = reader.ReadDestroyedCount();
            for (int c = 0; c < count; c += 1)
            {
                var entity = reader.ReadDestroyed(entityMapper);
                loader.OnDestroyedEntity(entity);
            }

            return this;
        }

    }
}