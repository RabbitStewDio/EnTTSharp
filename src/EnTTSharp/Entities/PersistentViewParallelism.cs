using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace EnTTSharp.Entities
{
    public static class PersistentViewParallelism
    {
        public static void PartitionAndRun<TEntityKey>(List<TEntityKey> p, Action<TEntityKey> action)
        {
            if (p.Count == 0)
            {
                return;
            }

            var rangeSize = Math.Max(1, p.Count / (Environment.ProcessorCount * 2));
            var partitioner = Partitioner.Create(0, p.Count, rangeSize);
            Parallel.ForEach(partitioner, range =>
            {
                var (min, max) = range;
                for (int i = min; i < max; i += 1)
                {
                    action(p[i]);
                }
            });
        }
    }
}