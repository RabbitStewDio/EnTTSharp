
//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

using EnttSharp.Entities.Helpers;

namespace EnttSharp.Entities
{

    public sealed class PersistentView<TEntityKey, T1> : PersistentMultiViewBase<TEntityKey>, IPersistentEntityView<TEntityKey, T1>
        where TEntityKey: IEntityKey
    {
        readonly Pools.Pool<TEntityKey, T1> pool1;

        public PersistentView(IEntityPoolAccess<TEntityKey> registry) :
            base(registry, 
                 registry.GetPool<T1>()
            )
        {
            pool1 = registry.GetPool<T1>();
        }

        public void Apply(ViewDelegates.Apply<TEntityKey, T1> bulk)
        {
            var p = EntityKeyListPool<TEntityKey>.Reserve(this.GetEnumerator(), EstimatedSize);
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
                EntityKeyListPool<TEntityKey>.Release(p);
            }
        }

        void ApplyOne(ViewDelegates.Apply<TEntityKey, T1> bulk, TEntityKey ek)
        {
            if (pool1.TryGet(ek, out var c1))
            {
                bulk(this, ek, in c1);
            }
        }

        public void ApplyWithContext<TContext>(TContext context, ViewDelegates.ApplyWithContext<TEntityKey, TContext, T1> bulk)
        {
            var p = EntityKeyListPool<TEntityKey>.Reserve(this.GetEnumerator(), EstimatedSize);
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
                EntityKeyListPool<TEntityKey>.Release(p);
            }
        }

        void ApplyOneWithContext<TContext>(TContext context, ViewDelegates.ApplyWithContext<TEntityKey, TContext, T1> bulk, TEntityKey ek)
        {
            if (pool1.TryGet(ek, out var c1))
            {
                bulk(this, context, ek, in c1);
            }
        }
    }

    public sealed class PersistentView<TEntityKey, T1, T2> : PersistentMultiViewBase<TEntityKey>, IPersistentEntityView<TEntityKey, T1, T2>
        where TEntityKey: IEntityKey
    {
        readonly Pools.Pool<TEntityKey, T1> pool1;
             readonly Pools.Pool<TEntityKey, T2> pool2;

        public PersistentView(IEntityPoolAccess<TEntityKey> registry) :
            base(registry, 
                 registry.GetPool<T1>(),
             registry.GetPool<T2>()
            )
        {
            pool1 = registry.GetPool<T1>();
             pool2 = registry.GetPool<T2>();
        }

        public void Apply(ViewDelegates.Apply<TEntityKey, T1, T2> bulk)
        {
            var p = EntityKeyListPool<TEntityKey>.Reserve(this.GetEnumerator(), EstimatedSize);
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
                EntityKeyListPool<TEntityKey>.Release(p);
            }
        }

        void ApplyOne(ViewDelegates.Apply<TEntityKey, T1, T2> bulk, TEntityKey ek)
        {
            if (pool1.TryGet(ek, out var c1) && 
             pool2.TryGet(ek, out var c2))
            {
                bulk(this, ek, in c1, in c2);
            }
        }

        public void ApplyWithContext<TContext>(TContext context, ViewDelegates.ApplyWithContext<TEntityKey, TContext, T1, T2> bulk)
        {
            var p = EntityKeyListPool<TEntityKey>.Reserve(this.GetEnumerator(), EstimatedSize);
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
                EntityKeyListPool<TEntityKey>.Release(p);
            }
        }

        void ApplyOneWithContext<TContext>(TContext context, ViewDelegates.ApplyWithContext<TEntityKey, TContext, T1, T2> bulk, TEntityKey ek)
        {
            if (pool1.TryGet(ek, out var c1) && 
             pool2.TryGet(ek, out var c2))
            {
                bulk(this, context, ek, in c1, in c2);
            }
        }
    }

    public sealed class PersistentView<TEntityKey, T1, T2, T3> : PersistentMultiViewBase<TEntityKey>, IPersistentEntityView<TEntityKey, T1, T2, T3>
        where TEntityKey: IEntityKey
    {
        readonly Pools.Pool<TEntityKey, T1> pool1;
             readonly Pools.Pool<TEntityKey, T2> pool2;
             readonly Pools.Pool<TEntityKey, T3> pool3;

        public PersistentView(IEntityPoolAccess<TEntityKey> registry) :
            base(registry, 
                 registry.GetPool<T1>(),
             registry.GetPool<T2>(),
             registry.GetPool<T3>()
            )
        {
            pool1 = registry.GetPool<T1>();
             pool2 = registry.GetPool<T2>();
             pool3 = registry.GetPool<T3>();
        }

        public void Apply(ViewDelegates.Apply<TEntityKey, T1, T2, T3> bulk)
        {
            var p = EntityKeyListPool<TEntityKey>.Reserve(this.GetEnumerator(), EstimatedSize);
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
                EntityKeyListPool<TEntityKey>.Release(p);
            }
        }

        void ApplyOne(ViewDelegates.Apply<TEntityKey, T1, T2, T3> bulk, TEntityKey ek)
        {
            if (pool1.TryGet(ek, out var c1) && 
             pool2.TryGet(ek, out var c2) && 
             pool3.TryGet(ek, out var c3))
            {
                bulk(this, ek, in c1, in c2, in c3);
            }
        }

        public void ApplyWithContext<TContext>(TContext context, ViewDelegates.ApplyWithContext<TEntityKey, TContext, T1, T2, T3> bulk)
        {
            var p = EntityKeyListPool<TEntityKey>.Reserve(this.GetEnumerator(), EstimatedSize);
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
                EntityKeyListPool<TEntityKey>.Release(p);
            }
        }

        void ApplyOneWithContext<TContext>(TContext context, ViewDelegates.ApplyWithContext<TEntityKey, TContext, T1, T2, T3> bulk, TEntityKey ek)
        {
            if (pool1.TryGet(ek, out var c1) && 
             pool2.TryGet(ek, out var c2) && 
             pool3.TryGet(ek, out var c3))
            {
                bulk(this, context, ek, in c1, in c2, in c3);
            }
        }
    }

    public sealed class PersistentView<TEntityKey, T1, T2, T3, T4> : PersistentMultiViewBase<TEntityKey>, IPersistentEntityView<TEntityKey, T1, T2, T3, T4>
        where TEntityKey: IEntityKey
    {
        readonly Pools.Pool<TEntityKey, T1> pool1;
             readonly Pools.Pool<TEntityKey, T2> pool2;
             readonly Pools.Pool<TEntityKey, T3> pool3;
             readonly Pools.Pool<TEntityKey, T4> pool4;

        public PersistentView(IEntityPoolAccess<TEntityKey> registry) :
            base(registry, 
                 registry.GetPool<T1>(),
             registry.GetPool<T2>(),
             registry.GetPool<T3>(),
             registry.GetPool<T4>()
            )
        {
            pool1 = registry.GetPool<T1>();
             pool2 = registry.GetPool<T2>();
             pool3 = registry.GetPool<T3>();
             pool4 = registry.GetPool<T4>();
        }

        public void Apply(ViewDelegates.Apply<TEntityKey, T1, T2, T3, T4> bulk)
        {
            var p = EntityKeyListPool<TEntityKey>.Reserve(this.GetEnumerator(), EstimatedSize);
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
                EntityKeyListPool<TEntityKey>.Release(p);
            }
        }

        void ApplyOne(ViewDelegates.Apply<TEntityKey, T1, T2, T3, T4> bulk, TEntityKey ek)
        {
            if (pool1.TryGet(ek, out var c1) && 
             pool2.TryGet(ek, out var c2) && 
             pool3.TryGet(ek, out var c3) && 
             pool4.TryGet(ek, out var c4))
            {
                bulk(this, ek, in c1, in c2, in c3, in c4);
            }
        }

        public void ApplyWithContext<TContext>(TContext context, ViewDelegates.ApplyWithContext<TEntityKey, TContext, T1, T2, T3, T4> bulk)
        {
            var p = EntityKeyListPool<TEntityKey>.Reserve(this.GetEnumerator(), EstimatedSize);
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
                EntityKeyListPool<TEntityKey>.Release(p);
            }
        }

        void ApplyOneWithContext<TContext>(TContext context, ViewDelegates.ApplyWithContext<TEntityKey, TContext, T1, T2, T3, T4> bulk, TEntityKey ek)
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

    public sealed class PersistentView<TEntityKey, T1, T2, T3, T4, T5> : PersistentMultiViewBase<TEntityKey>, IPersistentEntityView<TEntityKey, T1, T2, T3, T4, T5>
        where TEntityKey: IEntityKey
    {
        readonly Pools.Pool<TEntityKey, T1> pool1;
             readonly Pools.Pool<TEntityKey, T2> pool2;
             readonly Pools.Pool<TEntityKey, T3> pool3;
             readonly Pools.Pool<TEntityKey, T4> pool4;
             readonly Pools.Pool<TEntityKey, T5> pool5;

        public PersistentView(IEntityPoolAccess<TEntityKey> registry) :
            base(registry, 
                 registry.GetPool<T1>(),
             registry.GetPool<T2>(),
             registry.GetPool<T3>(),
             registry.GetPool<T4>(),
             registry.GetPool<T5>()
            )
        {
            pool1 = registry.GetPool<T1>();
             pool2 = registry.GetPool<T2>();
             pool3 = registry.GetPool<T3>();
             pool4 = registry.GetPool<T4>();
             pool5 = registry.GetPool<T5>();
        }

        public void Apply(ViewDelegates.Apply<TEntityKey, T1, T2, T3, T4, T5> bulk)
        {
            var p = EntityKeyListPool<TEntityKey>.Reserve(this.GetEnumerator(), EstimatedSize);
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
                EntityKeyListPool<TEntityKey>.Release(p);
            }
        }

        void ApplyOne(ViewDelegates.Apply<TEntityKey, T1, T2, T3, T4, T5> bulk, TEntityKey ek)
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

        public void ApplyWithContext<TContext>(TContext context, ViewDelegates.ApplyWithContext<TEntityKey, TContext, T1, T2, T3, T4, T5> bulk)
        {
            var p = EntityKeyListPool<TEntityKey>.Reserve(this.GetEnumerator(), EstimatedSize);
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
                EntityKeyListPool<TEntityKey>.Release(p);
            }
        }

        void ApplyOneWithContext<TContext>(TContext context, ViewDelegates.ApplyWithContext<TEntityKey, TContext, T1, T2, T3, T4, T5> bulk, TEntityKey ek)
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

    public sealed class PersistentView<TEntityKey, T1, T2, T3, T4, T5, T6> : PersistentMultiViewBase<TEntityKey>, IPersistentEntityView<TEntityKey, T1, T2, T3, T4, T5, T6>
        where TEntityKey: IEntityKey
    {
        readonly Pools.Pool<TEntityKey, T1> pool1;
             readonly Pools.Pool<TEntityKey, T2> pool2;
             readonly Pools.Pool<TEntityKey, T3> pool3;
             readonly Pools.Pool<TEntityKey, T4> pool4;
             readonly Pools.Pool<TEntityKey, T5> pool5;
             readonly Pools.Pool<TEntityKey, T6> pool6;

        public PersistentView(IEntityPoolAccess<TEntityKey> registry) :
            base(registry, 
                 registry.GetPool<T1>(),
             registry.GetPool<T2>(),
             registry.GetPool<T3>(),
             registry.GetPool<T4>(),
             registry.GetPool<T5>(),
             registry.GetPool<T6>()
            )
        {
            pool1 = registry.GetPool<T1>();
             pool2 = registry.GetPool<T2>();
             pool3 = registry.GetPool<T3>();
             pool4 = registry.GetPool<T4>();
             pool5 = registry.GetPool<T5>();
             pool6 = registry.GetPool<T6>();
        }

        public void Apply(ViewDelegates.Apply<TEntityKey, T1, T2, T3, T4, T5, T6> bulk)
        {
            var p = EntityKeyListPool<TEntityKey>.Reserve(this.GetEnumerator(), EstimatedSize);
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
                EntityKeyListPool<TEntityKey>.Release(p);
            }
        }

        void ApplyOne(ViewDelegates.Apply<TEntityKey, T1, T2, T3, T4, T5, T6> bulk, TEntityKey ek)
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

        public void ApplyWithContext<TContext>(TContext context, ViewDelegates.ApplyWithContext<TEntityKey, TContext, T1, T2, T3, T4, T5, T6> bulk)
        {
            var p = EntityKeyListPool<TEntityKey>.Reserve(this.GetEnumerator(), EstimatedSize);
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
                EntityKeyListPool<TEntityKey>.Release(p);
            }
        }

        void ApplyOneWithContext<TContext>(TContext context, ViewDelegates.ApplyWithContext<TEntityKey, TContext, T1, T2, T3, T4, T5, T6> bulk, TEntityKey ek)
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

    public sealed class PersistentView<TEntityKey, T1, T2, T3, T4, T5, T6, T7> : PersistentMultiViewBase<TEntityKey>, IPersistentEntityView<TEntityKey, T1, T2, T3, T4, T5, T6, T7>
        where TEntityKey: IEntityKey
    {
        readonly Pools.Pool<TEntityKey, T1> pool1;
             readonly Pools.Pool<TEntityKey, T2> pool2;
             readonly Pools.Pool<TEntityKey, T3> pool3;
             readonly Pools.Pool<TEntityKey, T4> pool4;
             readonly Pools.Pool<TEntityKey, T5> pool5;
             readonly Pools.Pool<TEntityKey, T6> pool6;
             readonly Pools.Pool<TEntityKey, T7> pool7;

        public PersistentView(IEntityPoolAccess<TEntityKey> registry) :
            base(registry, 
                 registry.GetPool<T1>(),
             registry.GetPool<T2>(),
             registry.GetPool<T3>(),
             registry.GetPool<T4>(),
             registry.GetPool<T5>(),
             registry.GetPool<T6>(),
             registry.GetPool<T7>()
            )
        {
            pool1 = registry.GetPool<T1>();
             pool2 = registry.GetPool<T2>();
             pool3 = registry.GetPool<T3>();
             pool4 = registry.GetPool<T4>();
             pool5 = registry.GetPool<T5>();
             pool6 = registry.GetPool<T6>();
             pool7 = registry.GetPool<T7>();
        }

        public void Apply(ViewDelegates.Apply<TEntityKey, T1, T2, T3, T4, T5, T6, T7> bulk)
        {
            var p = EntityKeyListPool<TEntityKey>.Reserve(this.GetEnumerator(), EstimatedSize);
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
                EntityKeyListPool<TEntityKey>.Release(p);
            }
        }

        void ApplyOne(ViewDelegates.Apply<TEntityKey, T1, T2, T3, T4, T5, T6, T7> bulk, TEntityKey ek)
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

        public void ApplyWithContext<TContext>(TContext context, ViewDelegates.ApplyWithContext<TEntityKey, TContext, T1, T2, T3, T4, T5, T6, T7> bulk)
        {
            var p = EntityKeyListPool<TEntityKey>.Reserve(this.GetEnumerator(), EstimatedSize);
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
                EntityKeyListPool<TEntityKey>.Release(p);
            }
        }

        void ApplyOneWithContext<TContext>(TContext context, ViewDelegates.ApplyWithContext<TEntityKey, TContext, T1, T2, T3, T4, T5, T6, T7> bulk, TEntityKey ek)
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