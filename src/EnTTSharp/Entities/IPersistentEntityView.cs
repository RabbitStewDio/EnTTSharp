namespace EnttSharp.Entities
{
    public interface IPersistentEntityView<T1> : IEntityView<T1>
    {
        int Count { get; }
    }

    public interface IPersistentEntityView<T1, T2> : IEntityView<T1, T2>
    {
        int Count { get; }
    }

    public interface IPersistentEntityView<T1, T2, T3> : IEntityView<T1, T2, T3>
    {
        int Count { get; }
    }

    public interface IPersistentEntityView<T1, T2, T3, T4> : IEntityView<T1, T2, T3, T4>
    {
        int Count { get; }
    }

    public interface IPersistentEntityView<T1, T2, T3, T4, T5> : IEntityView<T1, T2, T3, T4, T5>
    {
        int Count { get; }
    }

    public interface IPersistentEntityView<T1, T2, T3, T4, T5, T6> : IEntityView<T1, T2, T3, T4, T5, T6>
    {
        int Count { get; }
    }

    public interface IPersistentEntityView<T1, T2, T3, T4, T5, T6, T7> : IEntityView<T1, T2, T3, T4, T5, T6, T7>
    {
        int Count { get; }
    }
}