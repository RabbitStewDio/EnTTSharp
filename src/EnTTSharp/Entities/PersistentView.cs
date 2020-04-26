﻿using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;

namespace EnttSharp.Entities
{
    public sealed class PersistentView<T1> : PersistentMultiViewBase, IPersistentEntityView<T1>
    {
        readonly Pools.Pool<T1> pool1;

        public PersistentView(EntityRegistry registry) :
            base(registry, registry.GetPool<T1>())
        {
            pool1 = registry.GetPool<T1>();
        }

        public void Apply(ViewDelegates.Apply<T1> bulk)
        {
            var p = EntityKeyListPool.Reserve(this.GetEnumerator(), EstimatedSize);
            try
            {
                if (AllowParallelExecution)
                {
                    PersistentViewParallelism.PartitionAndRun(p, ek => ApplyOne(bulk, ek));
                }
                else
                {
                    foreach (var ek in p)
                    {
                        ApplyOne(bulk, ek);
                    }
                }
            }
            finally
            {
                EntityKeyListPool.Release(p);
            }
        }

        void ApplyOne(ViewDelegates.Apply<T1> bulk, EntityKey ek)
        {
            if (pool1.TryGet(ek, out var c1))
            {
                bulk(this, ek, in c1);
            }
        }

        public void ApplyWithContext<TContext>(TContext context, ViewDelegates.ApplyWithContext<TContext, T1> bulk)
        {
            var p = EntityKeyListPool.Reserve(this.GetEnumerator(), EstimatedSize);
            try
            {
                if (AllowParallelExecution)
                {
                    PersistentViewParallelism.PartitionAndRun(p, ek => ApplyOneWithContext(context, bulk, ek));
                }
                else
                {
                    foreach (var ek in p)
                    {
                        ApplyOneWithContext(context, bulk, ek);
                    }
                }
            }
            finally
            {
                EntityKeyListPool.Release(p);
            }
        }

        void ApplyOneWithContext<TContext>(TContext context, ViewDelegates.ApplyWithContext<TContext, T1> bulk, EntityKey ek)
        {
            if (pool1.TryGet(ek, out var c1))
            {
                bulk(this, context, ek, in c1);
            }
        }
    }

    public sealed class PersistentView<T1, T2> : PersistentMultiViewBase, IPersistentEntityView<T1, T2>
    {
        readonly Pools.Pool<T1> pool1;
        readonly Pools.Pool<T2> pool2;

        public PersistentView(EntityRegistry registry) :
            base(registry, registry.GetPool<T1>(), registry.GetPool<T2>())
        {
            pool1 = registry.GetPool<T1>();
            pool2 = registry.GetPool<T2>();
        }

        public void Apply(ViewDelegates.Apply<T1, T2> bulk)
        {
            var p = EntityKeyListPool.Reserve(this.GetEnumerator(), EstimatedSize);
            try
            {
                if (AllowParallelExecution)
                {
                    PersistentViewParallelism.PartitionAndRun(p, ek => ApplyOne(bulk, ek));
                }
                else
                {
                    foreach (var ek in p)
                    {
                        ApplyOne(bulk, ek);
                    }
                }
            }
            finally
            {
                EntityKeyListPool.Release(p);
            }
        }

        void ApplyOne(ViewDelegates.Apply<T1, T2> bulk, EntityKey ek)
        {
            if (pool1.TryGet(ek, out var c1) &&
                pool2.TryGet(ek, out var c2))
            {
                bulk(this, ek, in c1, in c2);
            }
        }

        public void ApplyWithContext<TContext>(TContext context, ViewDelegates.ApplyWithContext<TContext, T1, T2> bulk)
        {
            var p = EntityKeyListPool.Reserve(this.GetEnumerator(), EstimatedSize);
            try
            {
                if (AllowParallelExecution)
                {
                    PersistentViewParallelism.PartitionAndRun(p, ek => ApplyOneWithContext(context, bulk, ek));
                }
                else
                {
                    foreach (var ek in p)
                    {
                        ApplyOneWithContext(context, bulk, ek);
                    }
                }
            }
            finally
            {
                EntityKeyListPool.Release(p);
            }
        }

        void ApplyOneWithContext<TContext>(TContext context, ViewDelegates.ApplyWithContext<TContext, T1, T2> bulk, EntityKey ek)
        {
            if (pool1.TryGet(ek, out var c1) &&
                pool2.TryGet(ek, out var c2))
            {
                bulk(this, context, ek, in c1, in c2);
            }
        }
    }

    public sealed class PersistentView<T1, T2, T3> : PersistentMultiViewBase, IPersistentEntityView<T1, T2, T3>
    {
        readonly Pools.Pool<T1> pool1;
        readonly Pools.Pool<T2> pool2;
        readonly Pools.Pool<T3> pool3;

        public PersistentView(EntityRegistry registry) :
            base(registry,
                registry.GetPool<T1>(),
                registry.GetPool<T2>(),
                registry.GetPool<T3>())
        {
            pool1 = registry.GetPool<T1>();
            pool2 = registry.GetPool<T2>();
            pool3 = registry.GetPool<T3>();
        }

        public void Apply(ViewDelegates.Apply<T1, T2, T3> bulk)
        {
            var p = EntityKeyListPool.Reserve(this.GetEnumerator(), EstimatedSize);
            try
            {
                if (AllowParallelExecution)
                {
                    PersistentViewParallelism.PartitionAndRun(p, ek => ApplyOne(bulk, ek));
                }
                else
                {
                    foreach (var ek in p)
                    {
                        ApplyOne(bulk, ek);
                    }
                }
            }
            finally
            {
                EntityKeyListPool.Release(p);
            }
        }

        void ApplyOne(ViewDelegates.Apply<T1, T2, T3> bulk, EntityKey ek)
        {
            if (pool1.TryGet(ek, out var c1) &&
                pool2.TryGet(ek, out var c2) &&
                pool3.TryGet(ek, out var c3))
            {
                bulk(this, ek, in c1, in c2, in c3);
            }
        }

        public void ApplyWithContext<TContext>(TContext context,
                                               ViewDelegates.ApplyWithContext<TContext, T1, T2, T3> bulk)
        {
            var p = EntityKeyListPool.Reserve(this.GetEnumerator(), EstimatedSize);
            try
            {
                if (AllowParallelExecution)
                {
                    PersistentViewParallelism.PartitionAndRun(p, ek => ApplyOneWithContext(context, bulk, ek));
                }
                else
                {
                    foreach (var ek in p)
                    {
                        ApplyOneWithContext(context, bulk, ek);
                    }
                }
            }
            finally
            {
                EntityKeyListPool.Release(p);
            }
        }

        void ApplyOneWithContext<TContext>(TContext context, ViewDelegates.ApplyWithContext<TContext, T1, T2, T3> bulk, EntityKey ek)
        {
            if (pool1.TryGet(ek, out var c1) &&
                pool2.TryGet(ek, out var c2) &&
                pool3.TryGet(ek, out var c3))
            {
                bulk(this, context, ek, in c1, in c2, in c3);
            }
        }
    }

    public sealed class PersistentView<T1, T2, T3, T4> : PersistentMultiViewBase, IPersistentEntityView<T1, T2, T3, T4>
    {
        readonly Pools.Pool<T1> pool1;
        readonly Pools.Pool<T2> pool2;
        readonly Pools.Pool<T3> pool3;
        readonly Pools.Pool<T4> pool4;

        public PersistentView(EntityRegistry registry) :
            base(registry,
                registry.GetPool<T1>(),
                registry.GetPool<T2>(),
                registry.GetPool<T3>(),
                registry.GetPool<T4>())
        {
            pool1 = registry.GetPool<T1>();
            pool2 = registry.GetPool<T2>();
            pool3 = registry.GetPool<T3>();
            pool4 = registry.GetPool<T4>();
        }

        public void Apply(ViewDelegates.Apply<T1, T2, T3, T4> bulk)
        {
            var p = EntityKeyListPool.Reserve(this.GetEnumerator(), EstimatedSize);
            try
            {
                if (AllowParallelExecution)
                {
                    PersistentViewParallelism.PartitionAndRun(p, ek => ApplyOne(bulk, ek));
                }
                else
                {
                    foreach (var ek in p)
                    {
                        ApplyOne(bulk, ek);
                    }
                }
            }
            finally
            {
                EntityKeyListPool.Release(p);
            }
        }

        void ApplyOne(ViewDelegates.Apply<T1, T2, T3, T4> bulk, EntityKey ek)
        {
            if (pool1.TryGet(ek, out var c1) &&
                pool2.TryGet(ek, out var c2) &&
                pool3.TryGet(ek, out var c3) &&
                pool4.TryGet(ek, out var c4))
            {
                bulk(this, ek, in c1, in c2, in c3, in c4);
            }
        }

        public void ApplyWithContext<TContext>(TContext context,
                                               ViewDelegates.ApplyWithContext<TContext, T1, T2, T3, T4> bulk)
        {
            var p = EntityKeyListPool.Reserve(this.GetEnumerator(), EstimatedSize);
            try
            {
                if (AllowParallelExecution)
                {
                    PersistentViewParallelism.PartitionAndRun(p, ek => ApplyOneWithContext(context, bulk, ek));
                }
                else
                {
                    foreach (var ek in p)
                    {
                        ApplyOneWithContext(context, bulk, ek);
                    }
                }
            }
            finally
            {
                EntityKeyListPool.Release(p);
            }
        }

        void ApplyOneWithContext<TContext>(TContext context, ViewDelegates.ApplyWithContext<TContext, T1, T2, T3, T4> bulk, EntityKey ek)
        {
            if (pool1.TryGet(ek, out var c1) &&
                pool2.TryGet(ek, out var c2) &&
                pool3.TryGet(ek, out var c3) &&
                pool4.TryGet(ek, out var c4))
            {
                bulk(this, context, ek, in c1, in c2, in c3, in c4);
            }
        }
    }

    public sealed class PersistentView<T1, T2, T3, T4, T5> : PersistentMultiViewBase, IPersistentEntityView<T1, T2, T3, T4, T5>
    {
        readonly Pools.Pool<T1> pool1;
        readonly Pools.Pool<T2> pool2;
        readonly Pools.Pool<T3> pool3;
        readonly Pools.Pool<T4> pool4;
        readonly Pools.Pool<T5> pool5;

        public PersistentView(EntityRegistry registry) :
            base(registry,
                registry.GetPool<T1>(),
                registry.GetPool<T2>(),
                registry.GetPool<T3>(),
                registry.GetPool<T4>(),
                registry.GetPool<T5>())
        {
            pool1 = registry.GetPool<T1>();
            pool2 = registry.GetPool<T2>();
            pool3 = registry.GetPool<T3>();
            pool4 = registry.GetPool<T4>();
            pool5 = registry.GetPool<T5>();
        }

        public void Apply(ViewDelegates.Apply<T1, T2, T3, T4, T5> bulk)
        {
            var p = EntityKeyListPool.Reserve(this.GetEnumerator(), EstimatedSize);
            try
            {
                if (AllowParallelExecution)
                {
                    PersistentViewParallelism.PartitionAndRun(p, ek => ApplyOne(bulk, ek));
                }
                else
                {
                    foreach (var ek in p)
                    {
                        ApplyOne(bulk, ek);
                    }
                }
            }
            finally
            {
                EntityKeyListPool.Release(p);
            }
        }

        void ApplyOne(ViewDelegates.Apply<T1, T2, T3, T4, T5> bulk, EntityKey ek)
        {
            if (pool1.TryGet(ek, out var c1) &&
                pool2.TryGet(ek, out var c2) &&
                pool3.TryGet(ek, out var c3) &&
                pool4.TryGet(ek, out var c4) &&
                pool5.TryGet(ek, out var c5))
            {
                bulk(this, ek, in c1, in c2, in c3, in c4, in c5);
            }
        }

        public void ApplyWithContext<TContext>(TContext context,
                                               ViewDelegates.ApplyWithContext<TContext, T1, T2, T3, T4, T5> bulk)
        {
            var p = EntityKeyListPool.Reserve(this.GetEnumerator(), EstimatedSize);
            try
            {
                if (AllowParallelExecution)
                {
                    PersistentViewParallelism.PartitionAndRun(p, ek => ApplyOneWithContext(context, bulk, ek));
                }
                else
                {
                    foreach (var ek in p)
                    {
                        ApplyOneWithContext(context, bulk, ek);
                    }
                }
            }
            finally
            {
                EntityKeyListPool.Release(p);
            }
        }

        void ApplyOneWithContext<TContext>(TContext context, ViewDelegates.ApplyWithContext<TContext, T1, T2, T3, T4, T5> bulk, EntityKey ek)
        {
            if (pool1.TryGet(ek, out var c1) &&
                pool2.TryGet(ek, out var c2) &&
                pool3.TryGet(ek, out var c3) &&
                pool4.TryGet(ek, out var c4) &&
                pool5.TryGet(ek, out var c5))
            {
                bulk(this, context, ek, in c1, in c2, in c3, in c4, in c5);
            }
        }
    }

    public static class PersistentViewParallelism
    {
        public static void PartitionAndRun(List<EntityKey> p, Action<EntityKey> action)
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

    public sealed class PersistentView<T1, T2, T3, T4, T5, T6> : PersistentMultiViewBase,
                                                                 IPersistentEntityView<T1, T2, T3, T4, T5, T6>
    {
        readonly Pools.Pool<T1> pool1;
        readonly Pools.Pool<T2> pool2;
        readonly Pools.Pool<T3> pool3;
        readonly Pools.Pool<T4> pool4;
        readonly Pools.Pool<T5> pool5;
        readonly Pools.Pool<T6> pool6;

        public PersistentView(EntityRegistry registry) :
            base(registry,
                registry.GetPool<T1>(),
                registry.GetPool<T2>(),
                registry.GetPool<T3>(),
                registry.GetPool<T4>(),
                registry.GetPool<T5>(),
                registry.GetPool<T6>())
        {
            pool1 = registry.GetPool<T1>();
            pool2 = registry.GetPool<T2>();
            pool3 = registry.GetPool<T3>();
            pool4 = registry.GetPool<T4>();
            pool5 = registry.GetPool<T5>();
            pool6 = registry.GetPool<T6>();
        }

        public void Apply(ViewDelegates.Apply<T1, T2, T3, T4, T5, T6> bulk)
        {
            var p = EntityKeyListPool.Reserve(this.GetEnumerator(), EstimatedSize);
            try
            {
                if (AllowParallelExecution)
                {
                    PersistentViewParallelism.PartitionAndRun(p, ek => ApplyOne(bulk, ek));
                }
                else
                {
                    foreach (var ek in p)
                    {
                        ApplyOne(bulk, ek);
                    }
                }
            }
            finally
            {
                EntityKeyListPool.Release(p);
            }
        }

        void ApplyOne(ViewDelegates.Apply<T1, T2, T3, T4, T5, T6> bulk, EntityKey ek)
        {
            if (pool1.TryGet(ek, out var c1) &&
                pool2.TryGet(ek, out var c2) &&
                pool3.TryGet(ek, out var c3) &&
                pool4.TryGet(ek, out var c4) &&
                pool5.TryGet(ek, out var c5) &&
                pool6.TryGet(ek, out var c6))
            {
                bulk(this, ek, in c1, in c2, in c3, in c4, in c5, in c6);
            }
        }

        public void ApplyWithContext<TContext>(TContext context,
                                               ViewDelegates.ApplyWithContext<TContext, T1, T2, T3, T4, T5, T6> bulk)
        {
            var p = EntityKeyListPool.Reserve(this.GetEnumerator(), EstimatedSize);
            try
            {
                if (AllowParallelExecution)
                {
                    PersistentViewParallelism.PartitionAndRun(p, ek => ApplyOneWithContext(context, bulk, ek));
                }
                else
                {
                    foreach (var ek in p)
                    {
                        ApplyOneWithContext(context, bulk, ek);
                    }
                }
            }
            finally
            {
                EntityKeyListPool.Release(p);
            }
        }

        void ApplyOneWithContext<TContext>(TContext context, 
                                           ViewDelegates.ApplyWithContext<TContext, T1, T2, T3, T4, T5, T6> bulk, EntityKey ek)
        {
            if (pool1.TryGet(ek, out var c1) &&
                pool2.TryGet(ek, out var c2) &&
                pool3.TryGet(ek, out var c3) &&
                pool4.TryGet(ek, out var c4) &&
                pool5.TryGet(ek, out var c5) &&
                pool6.TryGet(ek, out var c6))
            {
                bulk(this, context, ek, in c1, in c2, in c3, in c4, in c5, in c6);
            }
        }
    }

    public sealed class PersistentView<T1, T2, T3, T4, T5, T6, T7> : PersistentMultiViewBase,
                                                                     IPersistentEntityView<T1, T2, T3, T4, T5, T6, T7>
    {
        readonly Pools.Pool<T1> pool1;
        readonly Pools.Pool<T2> pool2;
        readonly Pools.Pool<T3> pool3;
        readonly Pools.Pool<T4> pool4;
        readonly Pools.Pool<T5> pool5;
        readonly Pools.Pool<T6> pool6;
        readonly Pools.Pool<T7> pool7;

        public PersistentView(EntityRegistry registry) :
            base(registry,
                registry.GetPool<T1>(),
                registry.GetPool<T2>(),
                registry.GetPool<T3>(),
                registry.GetPool<T4>(),
                registry.GetPool<T5>(),
                registry.GetPool<T6>(),
                registry.GetPool<T7>())
        {
            pool1 = registry.GetPool<T1>();
            pool2 = registry.GetPool<T2>();
            pool3 = registry.GetPool<T3>();
            pool4 = registry.GetPool<T4>();
            pool5 = registry.GetPool<T5>();
            pool6 = registry.GetPool<T6>();
            pool7 = registry.GetPool<T7>();
        }

        public void Apply(ViewDelegates.Apply<T1, T2, T3, T4, T5, T6, T7> bulk)
        {
            var p = EntityKeyListPool.Reserve(this.GetEnumerator(), EstimatedSize);
            try
            {
                if (AllowParallelExecution)
                {
                    PersistentViewParallelism.PartitionAndRun(p, ek => ApplyOne(bulk, ek));
                }
                else
                {
                    foreach (var ek in p)
                    {
                        ApplyOne(bulk, ek);
                    }
                }
            }
            finally
            {
                EntityKeyListPool.Release(p);
            }
        }

        void ApplyOne(ViewDelegates.Apply<T1, T2, T3, T4, T5, T6, T7> bulk, EntityKey ek)
        {
            if (pool1.TryGet(ek, out var c1) &&
                pool2.TryGet(ek, out var c2) &&
                pool3.TryGet(ek, out var c3) &&
                pool4.TryGet(ek, out var c4) &&
                pool5.TryGet(ek, out var c5) &&
                pool6.TryGet(ek, out var c6) &&
                pool7.TryGet(ek, out var c7))
            {
                bulk(this, ek, in c1, in c2, in c3, in c4, in c5, in c6, in c7);
            }
        }

        public void ApplyWithContext<TContext>(TContext context,
                                               ViewDelegates.ApplyWithContext<TContext, T1, T2, T3, T4, T5, T6, T7>
                                                   bulk)
        {
            var p = EntityKeyListPool.Reserve(this.GetEnumerator(), EstimatedSize);
            try
            {
                if (AllowParallelExecution)
                {
                    PersistentViewParallelism.PartitionAndRun(p, ek => ApplyOneWithContext(context, bulk, ek));
                }
                else
                {
                    foreach (var ek in p)
                    {
                        ApplyOneWithContext(context, bulk, ek);
                    }
                }
            }
            finally
            {
                EntityKeyListPool.Release(p);
            }
        }

        void ApplyOneWithContext<TContext>(TContext context, ViewDelegates.ApplyWithContext<TContext, T1, T2, T3, T4, T5, T6, T7> bulk, EntityKey ek)
        {
            if (pool1.TryGet(ek, out var c1) &&
                pool2.TryGet(ek, out var c2) &&
                pool3.TryGet(ek, out var c3) &&
                pool4.TryGet(ek, out var c4) &&
                pool5.TryGet(ek, out var c5) &&
                pool6.TryGet(ek, out var c6) &&
                pool7.TryGet(ek, out var c7))
            {
                bulk(this, context, ek, in c1, in c2, in c3, in c4, in c5, in c6, in c7);
            }
        }
    }
}