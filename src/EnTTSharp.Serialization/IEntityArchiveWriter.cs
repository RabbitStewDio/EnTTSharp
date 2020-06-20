using EnttSharp.Entities;

namespace EnTTSharp.Serialization
{
    /// <summary>
    ///   Receiver interface for archived entity data. Data is written without buffering in the same order
    ///   as called by the SnapshotView.
    /// </summary>
    public interface IEntityArchiveWriter<TEntityKey>
        where TEntityKey: IEntityKey
    {
        void WriteStartEntity(in int entityCount);
        void WriteEntity(in TEntityKey entityKey);
        void WriteEndEntity();

        void WriteStartComponent<TComponent>(in int entityCount);
        void WriteComponent<TComponent>(in TEntityKey entityKey, in TComponent c);
        void WriteEndComponent<TComponent>();

        void WriteTag<TComponent>(in TEntityKey entityKey, in TComponent c);
        void WriteMissingTag<TComponent>();

        void WriteStartDestroyed(in int entityCount);
        void WriteDestroyed(in TEntityKey entityKey);
        void WriteEndDestroyed();

        void WriteEndOfFrame();
        void FlushFrame();
    }
}