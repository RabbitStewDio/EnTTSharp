using System;
using System.Reflection;
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

        public static (MethodInfo, MethodInfo) ReflectWriteHooks<TEntityKey>() where TEntityKey : IEntityKey
        {
            var writeComponentMethod = typeof(SnapshotView<TEntityKey>).GetMethod(nameof(SnapshotView<TEntityKey>.WriteComponent),
                                                                                  BindingFlags.Instance | BindingFlags.Public,
                                                                                  null,
                                                                                  new[] { typeof(IEntityArchiveWriter<TEntityKey>) },
                                                                                  null) ??
                                       throw new InvalidOperationException($"Unable to find required public method {nameof(SnapshotView<TEntityKey>.WriteComponent)}");

            var writeTagMethod = typeof(SnapshotView<TEntityKey>).GetMethod(nameof(SnapshotView<TEntityKey>.WriteTag),
                                                                            BindingFlags.Instance | BindingFlags.Public,
                                                                            null,
                                                                            new[] { typeof(IEntityArchiveWriter<TEntityKey>) },
                                                                            null) ??
                                 throw new InvalidOperationException($"Unable to find required public method {nameof(SnapshotView<TEntityKey>.WriteTag)}");

            return (writeComponentMethod, writeTagMethod);
        }


    }
}