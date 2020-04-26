using System;
using System.Collections.Concurrent;
using System.Collections.Generic;

namespace EnttSharp.Entities
{
    public static class EntityKeyListPool
    {
        static readonly ConcurrentQueue<List<EntityKey>> pools;

        static EntityKeyListPool()
        {
            pools = new ConcurrentQueue<List<EntityKey>>();
        }
        
        public static List<EntityKey> Reserve<TEnumerator>(TEnumerator src, int estimatedSize) where TEnumerator: IEnumerator<EntityKey>
        {
            if (!pools.TryDequeue(out var result))
            {
                result =  new List<EntityKey>(estimatedSize);
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

        public static void Release(List<EntityKey> l)
        {
            if (l == null) throw new ArgumentNullException(nameof(l));

            l.Clear();
            pools.Enqueue(l);
        }
    }
}