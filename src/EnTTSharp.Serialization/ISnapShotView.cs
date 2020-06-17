using System;

namespace EnTTSharp.Serialization
{
    public interface ISnapshotView : IDisposable
    {
        ISnapshotView WriteDestroyed(IEntityArchiveWriter writer);
        ISnapshotView WriteEntites(IEntityArchiveWriter writer);
        ISnapshotView WriteComponent<TComponent>(IEntityArchiveWriter writer);
        ISnapshotView WriteTag<TComponent>(IEntityArchiveWriter writer);
    }
}