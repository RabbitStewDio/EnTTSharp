using EnTTSharp.Entities;

namespace EnTTSharp.Serialization
{
    /// <summary>
    ///   An ordered stream loader.
    /// </summary>
    public class SnapshotStreamReader<TEntityKey> where TEntityKey : IEntityKey
    {
        readonly ISnapshotLoader<TEntityKey> loader;
        readonly EntityKeyMapper<TEntityKey> entityMapper;

        public SnapshotStreamReader(ISnapshotLoader<TEntityKey> loader)
        {
            this.loader = loader;
            this.entityMapper = loader.Map;
        }

        public SnapshotStreamReader<TEntityKey> ReadEntities(IEntityArchiveReader<TEntityKey> reader)
        {
            var count = reader.ReadEntityCount();
            for (int c = 0; c < count; c += 1)
            {
                var entity = reader.ReadEntity(entityMapper);
                loader.OnEntity(entity);
            }

            return this;
        }

        public SnapshotStreamReader<TEntityKey> ReadComponent<TComponent>(IEntityArchiveReader<TEntityKey> reader)
        {
            var count = reader.ReadComponentCount<TComponent>();
            for (int c = 0; c < count; c += 1)
            {
                if (reader.TryReadComponent(entityMapper, out TEntityKey entity, out TComponent component))
                {
                    loader.OnComponent(entity, component);
                }
            }

            return this;
        }

        public SnapshotStreamReader<TEntityKey> ReadTag<TComponent>(IEntityArchiveReader<TEntityKey> reader)
        {
            if (reader.ReadTagFlag<TComponent>())
            {
                if (reader.TryReadTag(entityMapper, out TEntityKey entity, out TComponent component))
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

        public SnapshotStreamReader<TEntityKey> ReadDestroyed(IEntityArchiveReader<TEntityKey> reader)
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