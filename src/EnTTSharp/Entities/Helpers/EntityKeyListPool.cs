using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using EnTTSharp.Entities.Pools;

namespace EnTTSharp.Entities.Helpers
{
    public static class EntityKeyListPool<TEntityKey> where TEntityKey : IEntityKey
    {
        static readonly ConcurrentQueue<List<TEntityKey>> pools;

        static EntityKeyListPool()
        {
            pools = new ConcurrentQueue<List<TEntityKey>>();
        }

        public static List<TEntityKey> Reserve(IEntityPoolAccess<TEntityKey> src) 
        {
            if (!pools.TryDequeue(out var result))
            {
                result = new List<TEntityKey>(src.Count);
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

        public static List<TEntityKey> Reserve(IEntityView<TEntityKey> src) 
        {
            if (!pools.TryDequeue(out var result))
            {
                result = new List<TEntityKey>(src.EstimatedSize);
            }
            else
            {
                result.Clear();
                result.Capacity = Math.Max(result.Capacity, src.EstimatedSize);
            }

            src.CopyTo(result);
            return result;
        }

        public static List<TEntityKey> Reserve(IReadOnlyPool<TEntityKey> src) 
        {
            if (!pools.TryDequeue(out var result))
            {
                result = new List<TEntityKey>(src.Count);
            }
            else
            {
                result.Clear();
                result.Capacity = Math.Max(result.Capacity, src.Count);
            }

            src.CopyTo(result);
            return result;
        }

        public static void Release(List<TEntityKey> l)
        {
            if (l == null) throw new ArgumentNullException(nameof(l));

            l.Clear();
            pools.Enqueue(l);
        }
    }

    public static class EntityKeyListPool
    {
        public static List<TEntityKey> Reserve<TEntityKey>(IEntityPoolAccess<TEntityKey> src) where TEntityKey : IEntityKey
        {
            return EntityKeyListPool<TEntityKey>.Reserve(src);
        }

        public static List<TEntityKey> Reserve<TEntityKey>(IEntityView<TEntityKey> src) where TEntityKey : IEntityKey
        {
            return EntityKeyListPool<TEntityKey>.Reserve(src);
        }

        public static List<TEntityKey> Reserve<TEntityKey>(IReadOnlyPool<TEntityKey> src) where TEntityKey : IEntityKey
        {
            return EntityKeyListPool<TEntityKey>.Reserve(src);
        }

        public static void Release<TEntityKey>(List<TEntityKey> l) where TEntityKey : IEntityKey
        {
            EntityKeyListPool<TEntityKey>.Release(l);
        }
    }
}