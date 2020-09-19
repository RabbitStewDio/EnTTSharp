using EnTTSharp.Entities;

namespace EnTTSharp.Serialization
{
    public interface IEntityArchiveReader<TEntityKey> 
        where TEntityKey: IEntityKey
    {
        int ReadEntityCount();
        TEntityKey ReadEntity(EntityKeyMapper<TEntityKey> entityMapper);

        int ReadComponentCount<TComponent>();
        bool TryReadComponent<TComponent>(EntityKeyMapper<TEntityKey> entityMapper, out TEntityKey key, out TComponent component);

        bool ReadTagFlag<TComponent>();
        bool TryReadTag<TComponent>(EntityKeyMapper<TEntityKey> entityMapper, out TEntityKey entityKey, out TComponent component);

        int ReadDestroyedCount();
        TEntityKey ReadDestroyed(EntityKeyMapper<TEntityKey> entityMapper);

    }
}