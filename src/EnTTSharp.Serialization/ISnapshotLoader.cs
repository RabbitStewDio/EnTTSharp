using EnttSharp.Entities;

namespace EnTTSharp.Serialization
{
    public interface ISnapshotLoader
    {
        void OnEntity(EntityKey entity);
        void OnDestroyedEntity(EntityKey entity);
        void OnComponent<TComponent>(EntityKey entity, in TComponent c);
        void OnTag<TComponent>(EntityKey entity, in TComponent c);
        EntityKey Map(EntityKey input);
        void CleanOrphans();
    }
}