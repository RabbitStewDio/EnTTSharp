using System.Threading.Tasks;
using EnttSharp.Entities;

namespace EnTTSharp.Serialization
{
    /// <summary>
    ///   Receiver interface for archived entity data. Data is written without buffering in the same order
    ///   as called by the SnapshotView.
    /// </summary>
    public interface IEntityArchiveWriter
    {
        void WriteStartEntity(in int entityCount);
        void WriteEntity(in EntityKey entityKey);
        void WriteEndEntity();

        void WriteStartComponent<TComponent>(in int entityCount);
        void WriteComponent<TComponent>(in EntityKey entityKey, in TComponent c);
        void WriteEndComponent<TComponent>();

        void WriteTag<TComponent>(in EntityKey entityKey, in TComponent c);
        void WriteMissingTag<TComponent>();

        void WriteStartDestroyed(in int entityCount);
        void WriteDestroyed(in EntityKey entityKey);
        void WriteEndDestroyed();

        void WriteEndOfFrame();
        void FlushFrame();
    }    
    
    public interface IAsyncEntityArchiveWriter
    {
        Task WriteStartEntityAsync(in int entityCount);
        Task WriteEntityAsync(in EntityKey entityKey);
        Task WriteEndEntityAsync();

        Task WriteStartComponentAsync<TComponent>(in int entityCount);
        Task WriteComponentAsync<TComponent>(in EntityKey entityKey, in TComponent c);
        Task WriteEndComponentAsync<TComponent>();

        Task WriteTagAsync<TComponent>(in EntityKey entityKey, in TComponent c);
        Task WriteMissingTagAsync<TComponent>();

        Task WriteStartDestroyedAsync(in int entityCount);
        Task WriteDestroyedAsync(in EntityKey entityKey);
        Task WriteEndDestroyedAsync();

        Task FlushFrameAsync();
    }
}