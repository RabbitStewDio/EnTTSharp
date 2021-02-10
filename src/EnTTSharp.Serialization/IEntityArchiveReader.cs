using EnTTSharp.Entities;

namespace EnTTSharp.Serialization
{
    public interface IEntityArchiveReader<TEntityKey> 
        where TEntityKey: IEntityKey
    {
        int ReadEntityCount();
        TEntityKey ReadEntity(IEntityKeyMapper entityMapper);

        int ReadComponentCount<TComponent>();
        bool TryReadComponent<TComponent>(IEntityKeyMapper entityMapper, out TEntityKey key, out TComponent component);

        bool ReadTagFlag<TComponent>();
        bool TryReadTag<TComponent>(IEntityKeyMapper entityMapper, out TEntityKey entityKey, out TComponent component);

        int ReadDestroyedCount();
        TEntityKey ReadDestroyed(IEntityKeyMapper entityMapper);

    }
}