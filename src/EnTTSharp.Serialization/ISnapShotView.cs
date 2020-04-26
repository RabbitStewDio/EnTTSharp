using System;

namespace EnTTSharp.Serialization
{
    public interface ISnapShotView : IDisposable
    {
        ISnapShotView WriteDestroyed(IEntityArchiveWriter writer);
        ISnapShotView WriteEntites(IEntityArchiveWriter writer);
        ISnapShotView WriteComponent<TComponent>(IEntityArchiveWriter writer);
        ISnapShotView WriteTag<TComponent>(IEntityArchiveWriter writer);
    }
}