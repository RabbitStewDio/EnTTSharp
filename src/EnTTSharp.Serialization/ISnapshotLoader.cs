using EnttSharp.Entities;

namespace EnTTSharp.Serialization
{
    /// <summary>
    ///   A callback interface that notifies the EntityRegistry of incoming changes.
    /// </summary>
    public interface ISnapshotLoader<TEntityKey>
        where TEntityKey : IEntityKey
    {
        void OnEntity(TEntityKey entity);
        void OnDestroyedEntity(TEntityKey entity);
        void OnComponent<TComponent>(TEntityKey entity, in TComponent c);
        void OnTag<TComponent>(TEntityKey entity, in TComponent c);
        TEntityKey Map(EntityKeyData input);
        void CleanOrphans();
        void OnTagRemoved<TComponent>();
    }
}