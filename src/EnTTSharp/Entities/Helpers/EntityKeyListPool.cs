using System;
using System.Collections.Concurrent;
using System.Collections.Generic;

namespace EnttSharp.Entities.Helpers
{
    public static class EntityKeyListPool<TEntityKey>
    {
        static readonly ConcurrentQueue<List<TEntityKey>> pools;

        static EntityKeyListPool()
        {
            pools = new ConcurrentQueue<List<TEntityKey>>();
        }

        public static List<TEntityKey> Reserve<TEnumerator>(TEnumerator src, 
                                                           int estimatedSize) 
            where TEnumerator : IEnumerator<TEntityKey>
        {
            if (!pools.TryDequeue(out var result))
            {
                result = new List<TEntityKey>(estimatedSize);
            }
            else
            {
                result.Clear();
                result.Capacity = Math.Max(result.Capacity, estimatedSize);
            }

            while (src.MoveNext())
            {
                result.Add(src.Current);
            }

            return result;
        }

        public static void Release(List<TEntityKey> l)
        {
            if (l == null) throw new ArgumentNullException(nameof(l));

            l.Clear();
            pools.Enqueue(l);
        }
    }
}