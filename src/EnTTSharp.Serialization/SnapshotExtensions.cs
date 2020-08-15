using EnttSharp.Entities;
using EnTTSharp.Entities;

namespace EnTTSharp.Serialization
{
    public static class SnapshotExtensions
    {
        public static SnapshotView<TEntityKey> CreateSnapshot<TEntityKey>(this Entities.EntityRegistry<TEntityKey> r) 
            where TEntityKey : IEntityKey
        {
            return new SnapshotView<TEntityKey>(r);
        }

        public static AsyncSnapshotView<TEntityKey> CreateAsyncSnapshot<TEntityKey>(this IEntityPoolAccess<TEntityKey> r)
            where TEntityKey : IEntityKey
        {
            return new AsyncSnapshotView<TEntityKey>(r);
        }

        public static SnapshotLoader<TEntityKey> CreateLoader<TEntityKey>(this Entities.EntityRegistry<TEntityKey> reg)
            where TEntityKey : IEntityKey
        {
            return new SnapshotLoader<TEntityKey>(reg);
        }
    }
}