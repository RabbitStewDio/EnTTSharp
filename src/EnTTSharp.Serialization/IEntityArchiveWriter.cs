using EnttSharp.Entities;

namespace EnTTSharp.Serialization
{
    public interface IEntityArchiveWriter
    {
        void WriteDestroyed(in EntityKey entityKey);
        void WriteEntity(in EntityKey entityKey);
        void WriteComponent<TComponent>(in EntityKey entityKey, in TComponent c);
        void WriteTag<TComponent>(in EntityKey entityKey, in TComponent c);
        void FlushFrame();
    }
}