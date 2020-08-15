using System;
using System.Collections;
using System.Collections.Generic;
using EnTTSharp.Entities.Helpers;
using EnTTSharp.Entities.Pools;

namespace EnTTSharp.Entities
{
    public sealed class AdhocView<TEntityKey, TComponent> : IEntityView<TEntityKey, TComponent> 
        where TEntityKey : IEntityKey
    {
        readonly IEntityPoolAccess<TEntityKey> registry;
        readonly IPool<TEntityKey, TComponent> viewData;
        readonly EventHandler<TEntityKey> onCreated;
        readonly EventHandler<TEntityKey> onDestroyed;
        bool disposed;
        public bool AllowParallelExecution { get; set; }

        public AdhocView(IEntityPoolAccess<TEntityKey> registry)
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

        public bool IsValid(TEntityKey entity)
        {
            return registry.IsValid(entity);
        }

        public bool IsOrphan(TEntityKey entity)
        {
            return registry.IsOrphan(entity);
        }

        public void Reset(TEntityKey entity)
        {
            registry.Reset(entity);
        }

        public bool HasTag<TTag>()
        {
            return registry.HasTag<TTag>();
        }

        public bool TryGetTag<TTag>(out TEntityKey k, out TTag tag)
        {
            return registry.TryGetTag(out k, out tag);
        }

        public void AttachTag<TTag>(TEntityKey entity)
        {
            registry.AttachTag<TTag>(entity);
        }

        public void AttachTag<TTag>(TEntityKey entity, in TTag tag)
        {
            registry.AttachTag(entity, in tag);
        }

        public void RemoveTag<TTag>()
        {
            registry.RemoveTag<TTag>();
        }

        public bool HasComponent<TOtherComponent>(TEntityKey entity)
        {
            return registry.HasComponent<TOtherComponent>(entity);
        }

        public TOtherComponent AssignComponent<TOtherComponent>(TEntityKey entity)
        {
            return registry.AssignComponent<TOtherComponent>(entity);
        }

        public void AssignComponent<TOtherComponent>(TEntityKey entity, in TOtherComponent c)
        {
            registry.AssignComponent(entity, in c);
        }

        public bool ReplaceComponent<TOtherComponent>(TEntityKey entity, in TOtherComponent c)
        {
            return registry.ReplaceComponent(entity, in c);
        }

        TOtherComponent IEntityViewControl<TEntityKey>.AssignOrReplace<TOtherComponent>(TEntityKey entity)
        {
            return registry.AssignOrReplace<TOtherComponent>(entity);
        }

        void OnCreated(object sender, TEntityKey e)
        {
            Created?.Invoke(sender, e);
        }

        void OnDestroyed(object sender, TEntityKey e)
        {
            Destroyed?.Invoke(sender, e);
        }

        public bool Contains(TEntityKey e)
        {
            return viewData.Contains(e);
        }

        public void Reserve(int capacity)
        {
            registry.GetPool<TComponent>().Reserve(capacity);
        }

        public void Respect<TOtherComponent>()
        {
            viewData.Respect(registry.GetPool<TOtherComponent>());
        }

        public bool GetComponent<TOtherComponent>(TEntityKey entity, out TOtherComponent data)
        {
            return registry.GetComponent(entity, out data);
        }

        public void WriteBack<TOtherComponent>(TEntityKey entity, in TOtherComponent data)
        {
            registry.WriteBack(entity, in data);
        }

        public void RemoveComponent<TOtherComponent>(TEntityKey entity)
        {
            registry.RemoveComponent<TOtherComponent>(entity);
        }

        public event EventHandler<TEntityKey> Destroyed;
        public event EventHandler<TEntityKey> Created;

        public void Apply(ViewDelegates.Apply<TEntityKey> bulk)
        {
            var p = EntityKeyListPool<TEntityKey>.Reserve(viewData.GetEnumerator(), viewData.Count);
            try
            {
                foreach (var ek in p)
                {
                    bulk(this, ek);
                }
            }
            finally
            {
                EntityKeyListPool<TEntityKey>.Release(p);
            }
        }

        public void ApplyWithContext<TContext>(TContext context, ViewDelegates.ApplyWithContext<TEntityKey, TContext> bulk)
        {
            var p = EntityKeyListPool<TEntityKey>.Reserve(viewData.GetEnumerator(), viewData.Count);
            try
            {
                foreach (var ek in p)
                {
                    bulk(this, context, ek);
                }
            }
            finally
            {
                EntityKeyListPool<TEntityKey>.Release(p);
            }
        }

        public void Apply(ViewDelegates.Apply<TEntityKey, TComponent> bulk)
        {
            var p = EntityKeyListPool<TEntityKey>.Reserve(viewData.GetEnumerator(), viewData.Count);
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
                EntityKeyListPool<TEntityKey>.Release(p);
            }
        }

        public void ApplyWithContext<TContext>(TContext context,
                                               ViewDelegates.ApplyWithContext<TEntityKey, TContext, TComponent> bulk)
        {
            var p = EntityKeyListPool<TEntityKey>.Reserve(viewData.GetEnumerator(), viewData.Count);
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
                EntityKeyListPool<TEntityKey>.Release(p);
            }
        }

        public void AssignOrReplace<TOtherComponent>(TEntityKey entity)
        {
            registry.AssignOrReplace<TOtherComponent>(entity);
        }

        public void AssignOrReplace<TOtherComponent>(TEntityKey entity, in TOtherComponent c)
        {
            registry.AssignOrReplace(entity, in c);
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        IEnumerator<TEntityKey> IEnumerable<TEntityKey>.GetEnumerator()
        {
            return GetEnumerator();
        }

        public RawList<TEntityKey>.Enumerator GetEnumerator()
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