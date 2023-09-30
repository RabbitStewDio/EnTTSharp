using System;
using System.Collections.Concurrent;
using EnTTSharp.Entities.Pools;

namespace EnTTSharp.Entities.Helpers
{
    public static class EntityKeyListPool<TEntityKey> where TEntityKey : IEntityKey
    {
        static readonly ConcurrentStack<RawList<TEntityKey>> Pools;

        static EntityKeyListPool()
        {
            Pools = new ConcurrentStack<RawList<TEntityKey>>();
        }

        public static RawList<TEntityKey> Reserve(IEntityPoolAccess<TEntityKey> src) 
        {
            if (!Pools.TryPop(out var result))
            {
                result = new RawList<TEntityKey>(src.Count);
            }
            else
            {
                result.Clear();
                result.Capacity = Math.Max(result.Capacity, src.Count);
            }

            foreach (var e in src)
            {
                result.Add(e);
            }
            return result;
        }

        public static RawList<TEntityKey> Reserve(IEntityView<TEntityKey> src) 
        {
            if (!Pools.TryPop(out var result))
            {
                result = new RawList<TEntityKey>(src.EstimatedSize);
            }
            else
            {
                result.Clear();
                result.Capacity = Math.Max(result.Capacity, src.EstimatedSize);
            }

            src.CopyTo(result);
            return result;
        }

        public static RawList<TEntityKey> Reserve(IReadOnlyPool<TEntityKey> src) 
        {
            if (!Pools.TryPop(out var result))
            {
                result = new RawList<TEntityKey>(src.Count);
            }
            else
            {
                result.Clear();
                result.Capacity = Math.Max(result.Capacity, src.Count);
            }

            src.CopyTo(result);
            return result;
        }

        public static void Release(RawList<TEntityKey> l)
        {
            if (l == null) throw new ArgumentNullException(nameof(l));

            l.Clear();
            Pools.Push(l);
        }
    }

    public static class EntityKeyListPool
    {
        public static RawList<TEntityKey> Reserve<TEntityKey>(IEntityPoolAccess<TEntityKey> src) where TEntityKey : IEntityKey
        {
            return EntityKeyListPool<TEntityKey>.Reserve(src);
        }

        public static RawList<TEntityKey> Reserve<TEntityKey>(IEntityView<TEntityKey> src) where TEntityKey : IEntityKey
        {
            return EntityKeyListPool<TEntityKey>.Reserve(src);
        }

        public static RawList<TEntityKey> Reserve<TEntityKey>(IReadOnlyPool<TEntityKey> src) where TEntityKey : IEntityKey
        {
            return EntityKeyListPool<TEntityKey>.Reserve(src);
        }

        public static void Release<TEntityKey>(RawList<TEntityKey> l) where TEntityKey : IEntityKey
        {
            EntityKeyListPool<TEntityKey>.Release(l);
        }
    }
}