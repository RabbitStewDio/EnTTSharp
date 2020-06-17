using System;
using System.Collections;
using System.Collections.Generic;
using EnttSharp.Entities.Helpers;

namespace EnttSharp.Entities
{
    public sealed class AdhocView<TComponent> : IEntityView<TComponent>
    {
        readonly Pools.Pool<TComponent> viewData;
        readonly EntityRegistry registry;
        readonly EventHandler<EntityKey> onCreated;
        readonly EventHandler<EntityKey> onDestroyed;
        bool disposed;
        public bool AllowParallelExecution { get; set; }

        public AdhocView(EntityRegistry registry)
        {
            this.registry = registry ?? throw new ArgumentNullException(nameof(registry));
            this.viewData = registry.GetPool<TComponent>();
            onCreated = OnCreated;
            onDestroyed = OnDestroyed;
            this.viewData.Destroyed += onDestroyed;
            this.viewData.Created += onCreated;
        }

        ~AdhocView()
        {
            Dispose(false);
        }

        public bool IsOrphan(EntityKey entity)
        {
            return registry.IsOrphan(entity);
        }

        public void Reset(EntityKey entity)
        {
            registry.Reset(entity);
        }

        public bool HasTag<TTag>()
        {
            return registry.HasTag<TTag>();
        }

        public bool TryGetTag<TTag>(out EntityKey k, out TTag tag)
        {
            return registry.TryGetTag(out k, out tag);
        }

        public void AttachTag<TTag>(EntityKey entity)
        {
            registry.AttachTag<TTag>(entity);
        }

        public void AttachTag<TTag>(EntityKey entity, in TTag tag)
        {
            registry.AttachTag(entity, in tag);
        }

        public void RemoveTag<TTag>()
        {
            registry.RemoveTag<TTag>();
        }

        public bool HasComponent<TOtherComponent>(EntityKey entity)
        {
            return registry.HasComponent<TOtherComponent>(entity);
        }

        public TOtherComponent AssignComponent<TOtherComponent>(EntityKey entity)
        {
            return registry.AssignComponent<TOtherComponent>(entity);
        }

        public void AssignComponent<TOtherComponent>(EntityKey entity, in TOtherComponent c)
        {
            registry.AssignComponent(entity, in c);
        }

        public bool ReplaceComponent<TOtherComponent>(EntityKey entity, in TOtherComponent c)
        {
            return registry.ReplaceComponent(entity, in c);
        }

        TOtherComponent IEntityViewControl.AssignOrReplace<TOtherComponent>(EntityKey entity)
        {
            return registry.AssignOrReplace<TOtherComponent>(entity);
        }

        void OnCreated(object sender, EntityKey e)
        {
            Created?.Invoke(sender, e);
        }

        void OnDestroyed(object sender, EntityKey e)
        {
            Destroyed?.Invoke(sender, e);
        }

        public bool Contains(EntityKey e)
        {
            return viewData.Contains(e);
        }

        public void Reserve(int capacity)
        {
            registry.Reserve<TComponent>(capacity);
        }

        public void Respect<TOtherComponent>()
        {
            viewData.Respect(registry.View<TOtherComponent>());
        }

        public bool GetComponent<TOtherComponent>(EntityKey entity, out TOtherComponent data)
        {
            return registry.GetComponent(entity, out data);
        }

        public void WriteBack<TOtherComponent>(EntityKey entity, in TOtherComponent data)
        {
            registry.WriteBack(entity, in data);
        }

        public void RemoveComponent<TOtherComponent>(EntityKey entity)
        {
            registry.RemoveComponent<TOtherComponent>(entity);
        }

        public event EventHandler<EntityKey> Destroyed;
        public event EventHandler<EntityKey> Created;

        public void Apply(ViewDelegates.Apply bulk)
        {
            var p = EntityKeyListPool.Reserve(viewData.GetEnumerator(), viewData.Count);
            try
            {
                foreach (var ek in p)
                {
                    bulk(this, ek);
                }
            }
            finally
            {
                EntityKeyListPool.Release(p);
            }
        }

        public void ApplyWithContext<TContext>(TContext context, ViewDelegates.ApplyWithContext<TContext> bulk)
        {
            var p = EntityKeyListPool.Reserve(viewData.GetEnumerator(), viewData.Count);
            try
            {
                foreach (var ek in p)
                {
                    bulk(this, context, ek);
                }
            }
            finally
            {
                EntityKeyListPool.Release(p);
            }
        }

        public void Apply(ViewDelegates.Apply<TComponent> bulk)
        {
            var p = EntityKeyListPool.Reserve(viewData.GetEnumerator(), viewData.Count);
            try
            {
                foreach (var ek in p)
                {
                    if (viewData.TryGet(ek, out var c))
                    {
                        bulk(this, ek, in c);
                    }
                }
            }
            finally
            {
                EntityKeyListPool.Release(p);
            }
        }

        public void ApplyWithContext<TContext>(TContext context,
                                               ViewDelegates.ApplyWithContext<TContext, TComponent> bulk)
        {
            var p = EntityKeyListPool.Reserve(viewData.GetEnumerator(), viewData.Count);
            try
            {
                foreach (var ek in p)
                {
                    if (viewData.TryGet(ek, out var c))
                    {
                        bulk(this, context, ek, in c);
                    }
                }
            }
            finally
            {
                EntityKeyListPool.Release(p);
            }
        }

        public void AssignOrReplace<TOtherComponent>(EntityKey entity)
        {
            registry.AssignOrReplace<TOtherComponent>(entity);
        }

        public void AssignOrReplace<TOtherComponent>(EntityKey entity, in TOtherComponent c)
        {
            registry.AssignOrReplace(entity, in c);
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        IEnumerator<EntityKey> IEnumerable<EntityKey>.GetEnumerator()
        {
            return GetEnumerator();
        }

        public RawList<EntityKey>.Enumerator GetEnumerator()
        {
            return viewData.GetEnumerator();
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        void Dispose(bool disposing)
        {
            if (disposed)
            {
                return;
            }

            disposed = true;
            this.viewData.Destroyed -= onDestroyed;
            this.viewData.Created -= onCreated;
        }
    }
}