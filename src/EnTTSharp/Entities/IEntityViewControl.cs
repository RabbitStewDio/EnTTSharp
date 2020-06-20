namespace EnttSharp.Entities
{
    /// <summary>
    ///   A controller interface that allows actions to query and update
    ///   components.
    /// </summary>
    public interface IEntityViewControl<TEntityKey>
        where TEntityKey: IEntityKey
    {
        bool HasTag<TTag>();
        bool TryGetTag<TTag>(out TEntityKey k, out TTag tag);
        void RemoveTag<TTag>();
        void AttachTag<TTag>(TEntityKey entity);
        void AttachTag<TTag>(TEntityKey entity, in TTag tag);

        bool Contains(TEntityKey e);
        bool GetComponent<TComponent>(TEntityKey entity, out TComponent data);
        bool HasComponent<TComponent>(TEntityKey entity);

        // Writeback is only necessary for structs. Classes are by-ref and all references point to the
        // same instance anyway.
        void WriteBack<TComponent>(TEntityKey entity, in TComponent data);
        void RemoveComponent<TComponent>(TEntityKey entity);

        TComponent AssignComponent<TComponent>(TEntityKey entity);
        void AssignComponent<TComponent>(TEntityKey entity, in TComponent c);

        TComponent AssignOrReplace<TComponent>(TEntityKey entity);
        void AssignOrReplace<TComponent>(TEntityKey entity, in TComponent c);
        bool ReplaceComponent<TComponent>(TEntityKey entity, in TComponent c);

        void Reset(TEntityKey entity);
        bool IsOrphan(TEntityKey entity);
    }
}