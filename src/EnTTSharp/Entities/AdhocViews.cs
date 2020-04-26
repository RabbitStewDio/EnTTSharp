namespace EnttSharp.Entities
{
    public sealed class AdhocView<T1, T2> : AdhocMultiViewBase, IEntityView<T1, T2>
    {
        readonly Pools.Pool<T1> pool1;
        readonly Pools.Pool<T2> pool2;

        public AdhocView(EntityRegistry registry) :
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
                foreach (var ek in p)
                {
                    if (pool1.TryGet(ek, out var c1) &&
                        pool2.TryGet(ek, out var c2))
                    {
                        bulk(this, ek, in c1, in c2);
                    }
                }
            }
            finally
            {
                EntityKeyListPool.Release(p);
            }
        }

        public void ApplyWithContext<TContext>(TContext context, ViewDelegates.ApplyWithContext<TContext, T1, T2> bulk)
        {
            var p = EntityKeyListPool.Reserve(this.GetEnumerator(), EstimatedSize);
            try
            {
                foreach (var ek in p)
                {
                    if (pool1.TryGet(ek, out var c1) &&
                        pool2.TryGet(ek, out var c2))
                    {
                        bulk(this, context, ek, in c1, in c2);
                    }
                }
            }
            finally
            {
                EntityKeyListPool.Release(p);
            }
        }
    }

    public sealed class AdhocView<T1, T2, T3> : AdhocMultiViewBase, IEntityView<T1, T2, T3>
    {
        readonly Pools.Pool<T1> pool1;
        readonly Pools.Pool<T2> pool2;
        readonly Pools.Pool<T3> pool3;

        public AdhocView(EntityRegistry registry) :
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
                foreach (var ek in p)
                {
                    if (pool1.TryGet(ek, out var c1) &&
                        pool2.TryGet(ek, out var c2) &&
                        pool3.TryGet(ek, out var c3))
                    {
                        bulk(this, ek, in c1, in c2, in c3);
                    }
                }
            }
            finally
            {
                EntityKeyListPool.Release(p);
            }
        }

        public void ApplyWithContext<TContext>(TContext context,
                                               ViewDelegates.ApplyWithContext<TContext, T1, T2, T3> bulk)
        {
            var p = EntityKeyListPool.Reserve(this.GetEnumerator(), EstimatedSize);
            try
            {
                foreach (var ek in p)
                {
                    if (pool1.TryGet(ek, out var c1) &&
                        pool2.TryGet(ek, out var c2) &&
                        pool3.TryGet(ek, out var c3))
                    {
                        bulk(this, context, ek, in c1, in c2, in c3);
                    }
                }
            }
            finally
            {
                EntityKeyListPool.Release(p);
            }
        }
    }

    public sealed class AdhocView<T1, T2, T3, T4> : AdhocMultiViewBase, IEntityView<T1, T2, T3, T4>
    {
        readonly Pools.Pool<T1> pool1;
        readonly Pools.Pool<T2> pool2;
        readonly Pools.Pool<T3> pool3;
        readonly Pools.Pool<T4> pool4;

        public AdhocView(EntityRegistry registry) :
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
                foreach (var ek in p)
                {
                    if (pool1.TryGet(ek, out var c1) &&
                        pool2.TryGet(ek, out var c2) &&
                        pool3.TryGet(ek, out var c3) &&
                        pool4.TryGet(ek, out var c4))
                    {
                        bulk(this, ek, in c1, in c2, in c3, in c4);
                    }
                }
            }
            finally
            {
                EntityKeyListPool.Release(p);
            }
        }

        public void ApplyWithContext<TContext>(TContext context,
                                               ViewDelegates.ApplyWithContext<TContext, T1, T2, T3, T4> bulk)
        {
            var p = EntityKeyListPool.Reserve(this.GetEnumerator(), EstimatedSize);
            try
            {
                foreach (var ek in p)
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
            finally
            {
                EntityKeyListPool.Release(p);
            }
        }
    }

    public sealed class AdhocView<T1, T2, T3, T4, T5> : AdhocMultiViewBase, IEntityView<T1, T2, T3, T4, T5>
    {
        readonly Pools.Pool<T1> pool1;
        readonly Pools.Pool<T2> pool2;
        readonly Pools.Pool<T3> pool3;
        readonly Pools.Pool<T4> pool4;
        readonly Pools.Pool<T5> pool5;

        public AdhocView(EntityRegistry registry) :
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
                foreach (var ek in p)
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
            }
            finally
            {
                EntityKeyListPool.Release(p);
            }
        }

        public void ApplyWithContext<TContext>(TContext context,
                                               ViewDelegates.ApplyWithContext<TContext, T1, T2, T3, T4, T5> bulk)
        {
            var p = EntityKeyListPool.Reserve(this.GetEnumerator(), EstimatedSize);
            try
            {
                foreach (var ek in p)
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
            finally
            {
                EntityKeyListPool.Release(p);
            }
        }
    }

    public sealed class AdhocView<T1, T2, T3, T4, T5, T6> : AdhocMultiViewBase, IEntityView<T1, T2, T3, T4, T5, T6>
    {
        readonly Pools.Pool<T1> pool1;
        readonly Pools.Pool<T2> pool2;
        readonly Pools.Pool<T3> pool3;
        readonly Pools.Pool<T4> pool4;
        readonly Pools.Pool<T5> pool5;
        readonly Pools.Pool<T6> pool6;

        public AdhocView(EntityRegistry registry) :
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
                foreach (var ek in p)
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
            }
            finally
            {
                EntityKeyListPool.Release(p);
            }
        }

        public void ApplyWithContext<TContext>(TContext context,
                                               ViewDelegates.ApplyWithContext<TContext, T1, T2, T3, T4, T5, T6> bulk)
        {
            var p = EntityKeyListPool.Reserve(this.GetEnumerator(), EstimatedSize);
            try
            {
                foreach (var ek in p)
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
            finally
            {
                EntityKeyListPool.Release(p);
            }
        }
    }

    public sealed class AdhocView<T1, T2, T3, T4, T5, T6, T7> : AdhocMultiViewBase,
                                                                IEntityView<T1, T2, T3, T4, T5, T6, T7>
    {
        readonly Pools.Pool<T1> pool1;
        readonly Pools.Pool<T2> pool2;
        readonly Pools.Pool<T3> pool3;
        readonly Pools.Pool<T4> pool4;
        readonly Pools.Pool<T5> pool5;
        readonly Pools.Pool<T6> pool6;
        readonly Pools.Pool<T7> pool7;

        public AdhocView(EntityRegistry registry) :
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
                foreach (var ek in p)
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
            }
            finally
            {
                EntityKeyListPool.Release(p);
            }
        }

        public void ApplyWithContext<TContext>(TContext context,
                                               ViewDelegates.ApplyWithContext<TContext, T1, T2, T3, T4, T5, T6, T7>
                                                   bulk)
        {
            var p = EntityKeyListPool.Reserve(this.GetEnumerator(), EstimatedSize);
            try
            {
                foreach (var ek in p)
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
            finally
            {
                EntityKeyListPool.Release(p);
            }
        }
    }
}