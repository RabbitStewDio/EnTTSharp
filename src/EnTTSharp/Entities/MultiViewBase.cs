using System;
using System.Collections;
using System.Collections.Generic;

namespace EnttSharp.Entities
{
    public abstract class MultiViewBase<TEnumerator> : IEntityView where TEnumerator : IEnumerator<EntityKey>
    {
        readonly EntityRegistry registry;
        readonly EventHandler<EntityKey> onCreated;
        readonly EventHandler<EntityKey> onDestroyed;
        protected readonly List<ISparsePool> Sets;
        /// <summary>
        ///   Use this as a general fallback during the construction of subclasses, where it may not yet
        ///   be safe to use the overloaded 'Contains' method.
        /// </summary>
        protected readonly Func<EntityKey, bool> IsMemberPredicate;

        public bool AllowParallelExecution { get; set; }

        protected MultiViewBase(EntityRegistry registry,
                                IReadOnlyList<ISparsePool> entries)
        {
            this.registry = registry ?? throw new ArgumentNullException(nameof(registry));
            if (entries == null || entries.Count == 0)
            {
                throw new ArgumentException();
            }

            onCreated = OnCreated;
            onDestroyed = OnDestroyed;
            this.Sets = new List<ISparsePool>(entries);
            foreach (var pool in Sets)
            {
                pool.Destroyed += onDestroyed;
                pool.Created += onCreated;
            }

            IsMemberPredicate = IsMember;
        }

        ~MultiViewBase()
        {
            Disposing(false);
        }

        public event EventHandler<EntityKey> Destroyed;
        public event EventHandler<EntityKey> Created;

        protected virtual void OnCreated(object sender, EntityKey e)
        {
            if (Contains(e))
            {
                Created?.Invoke(sender, e);
            }
        }

        protected virtual void OnDestroyed(object sender, EntityKey e)
        {
            var countContained = 0;
            foreach (var pool in Sets)
            {
                if (pool.Contains(e))
                {
                    countContained += 1;
                }
            }

            if (countContained == Sets.Count - 1)
            {
                Destroyed?.Invoke(sender, e);
            }
        }

        public void Reserve(int capacity)
        {
            foreach (var pool in Sets)
            {
                pool.Reserve(capacity);
            }
        }

        protected EntityRegistry Registry
        {
            get { return registry; }
        }

        public void RemoveComponent<TComponent>(EntityKey entity)
        {
            registry.RemoveComponent<TComponent>(entity);
        }

        public bool ReplaceComponent<TComponent>(EntityKey entity, in TComponent c)
        {
            return registry.ReplaceComponent(entity, in c);
        }

        public virtual void Respect<TComponent>()
        {
            // adhoc views ignore the respect command. 
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        IEnumerator<EntityKey> IEnumerable<EntityKey>.GetEnumerator()
        {
            return GetEnumerator();
        }

        public abstract TEnumerator GetEnumerator();

        protected abstract int EstimatedSize { get; }
        
        protected bool IsMember(EntityKey e)
        {
            foreach (var set in Sets)
            {
                if (!set.Contains(e))
                {
                    return false;
                }
            }

            return true;
        }

        public void Reset(EntityKey entity)
        {
            registry.Reset(entity);
        }

        public virtual bool Contains(EntityKey e)
        {
            return IsMember(e);
        }

        public bool IsOrphan(EntityKey entity)
        {
            return registry.IsOrphan(entity);
        }

        public void Apply(ViewDelegates.Apply bulk)
        {
            var p = EntityKeyListPool.Reserve(this.GetEnumerator(), EstimatedSize);
            try
            {
                foreach (var e in p)
                {
                    bulk(this, e);
                }
            }
            finally
            {
                EntityKeyListPool.Release(p);
            }
        }

        public void ApplyWithContext<TContext>(TContext c, ViewDelegates.ApplyWithContext<TContext> bulk)
        {
            var p = EntityKeyListPool.Reserve(this.GetEnumerator(), EstimatedSize);
            try
            {
                foreach (var e in p)
                {
                    bulk(this, c, e);
                }
            }
            finally
            {
                EntityKeyListPool.Release(p);
            }

        }

        public bool GetComponent<TComponent>(EntityKey entity, out TComponent data)
        {
            return registry.GetComponent(entity, out data);
        }

        public void WriteBack<TComponent>(EntityKey entity, in TComponent data)
        {
            registry.WriteBack(entity, in data);
        }

        public void AssignOrReplace<TComponent>(EntityKey entity)
        {
            registry.AssignOrReplace<TComponent>(entity);
        }

        public void AssignOrReplace<TComponent>(EntityKey entity, TComponent c)
        {
            registry.AssignOrReplace(entity, c);
        }

        public void AssignOrReplace<TComponent>(EntityKey entity, in TComponent c)
        {
            registry.AssignOrReplace(entity, in c);
        }

        public bool HasTag<TTag>()
        {
            return registry.HasTag<TTag>();
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

        public bool TryGetTag<TTag>(out EntityKey k, out TTag tag)
        {
            return registry.TryGetTag(out k, out tag);
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

        TOtherComponent IEntityViewControl.AssignOrReplace<TOtherComponent>(EntityKey entity)
        {
            return registry.AssignOrReplace<TOtherComponent>(entity);
        }

        public void Dispose()
        {
            Disposing(true);
            GC.SuppressFinalize(this);
        }

        protected bool Disposed { get; private set; }

        protected virtual void Disposing(bool disposing)
        {
            if (Disposed)
            {
                return;
            }

            Disposed = true;
            foreach (var pool in Sets)
            {
                pool.Destroyed -= onDestroyed;
                pool.Created -= onCreated;
            }
        }
    }
}