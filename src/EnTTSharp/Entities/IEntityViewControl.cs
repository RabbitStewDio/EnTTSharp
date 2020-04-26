namespace EnttSharp.Entities
{
    /// <summary>
    ///   A controller interface that allows actions to query and update
    ///   components.
    /// </summary>
    public interface IEntityViewControl
    {
        bool HasTag<TTag>();
        bool TryGetTag<TTag>(out EntityKey k, out TTag tag);
        void RemoveTag<TTag>();
        void AttachTag<TTag>(EntityKey entity);
        void AttachTag<TTag>(EntityKey entity, in TTag tag);

        bool Contains(EntityKey e);
        bool GetComponent<TComponent>(EntityKey entity, out TComponent data);
        bool HasComponent<TComponent>(EntityKey entity);

        // Writeback is only necessary for structs. Classes are by-ref and all references point to the
        // same instance anyway.
        void WriteBack<TComponent>(EntityKey entity, in TComponent data);
        void RemoveComponent<TComponent>(EntityKey entity);

        TComponent AssignComponent<TComponent>(EntityKey entity);
        void AssignComponent<TComponent>(EntityKey entity, in TComponent c);

        TComponent AssignOrReplace<TComponent>(EntityKey entity);
        void AssignOrReplace<TComponent>(EntityKey entity, in TComponent c);
        bool ReplaceComponent<TComponent>(EntityKey entity, in TComponent c);

        void Reset(EntityKey entity);
        bool IsOrphan(EntityKey entity);
    }
}