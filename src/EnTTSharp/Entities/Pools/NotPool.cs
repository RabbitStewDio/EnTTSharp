using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using EnTTSharp.Entities.Helpers;

namespace EnTTSharp.Entities.Pools
{
    public class NotPool<TEntityKey, TComponent>: IReadOnlyPool<TEntityKey, Not<TComponent>>, IDisposable
        where TEntityKey : IEntityKey
    {
        readonly IEntityPoolAccess<TEntityKey> registry;
        readonly IReadOnlyPool<TEntityKey, TComponent> entityPool;

        public NotPool(IEntityPoolAccess<TEntityKey> registry, 
                       IReadOnlyPool<TEntityKey, TComponent> entityPool)
        {
            this.registry = registry;
            this.registry.BeforeEntityDestroyed += HandleEntityDestroyed;
            this.registry.EntityCreated += HandleEntityCreated;
            this.entityPool = entityPool ?? throw new ArgumentNullException(nameof(entityPool));
            this.entityPool.CreatedEntry += HandleCreated;
            this.entityPool.Destroyed += HandleDestroyed;
        }

        void HandleEntityCreated(object sender, TEntityKey e)
        { 
            CreatedEntry?.Invoke(this, e);
            Created?.Invoke(this, (e, default));
        }

        void HandleEntityDestroyed(object sender, TEntityKey e)
        {
            Destroyed?.Invoke(this, e);
            DestroyedNotify?.Invoke(this, (e, default));
        }

        public void Dispose()
        {
            this.entityPool.CreatedEntry -= HandleCreated;
            this.entityPool.Destroyed -= HandleCreated;
        }

        void HandleCreated(object sender, TEntityKey e)
        {
            Destroyed?.Invoke(this, e);
            DestroyedNotify?.Invoke(this, (e, default));
        }

        void HandleDestroyed(object sender, TEntityKey e)
        {
            CreatedEntry?.Invoke(this, e);
            Created?.Invoke(this, (e, default));
        }

        public int Count => registry.Count - entityPool.Count;

        public event EventHandler<TEntityKey>? Destroyed;
        public event EventHandler<TEntityKey>? UpdatedEntry
        {
            add { }
            remove { }
        }
        
        public event EventHandler<TEntityKey>? CreatedEntry;
        public event EventHandler<(TEntityKey key, Not<TComponent> old)>? DestroyedNotify;
        public event EventHandler<(TEntityKey key, Not<TComponent> old)>? Updated
        {
            add { }
            remove { }
        }
        
        public event EventHandler<(TEntityKey key, Not<TComponent> old)>? Created;
        
        public bool TryGet(TEntityKey entity, out Not<TComponent> component)
        {
            component = default;
            return Contains(entity);
        }

        public bool Contains(TEntityKey k)
        {
            return registry.IsValid(k) && !entityPool.Contains(k);
        }

        [SuppressMessage("ReSharper", "RedundantAssignment")]
        public ref readonly Not<TComponent> TryGetRef(TEntityKey entity, ref Not<TComponent> defaultValue, out bool success)
        {
            success = Contains(entity);
            return ref defaultValue;
        }

        public void Reserve(int capacity)
        {
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        IEnumerator<TEntityKey> IEnumerable<TEntityKey>.GetEnumerator()
        {
            return GetEnumerator();
        }

        public Enumerator GetEnumerator() => new Enumerator(this);

        public void CopyTo(RawList<TEntityKey> entities)
        {
            entities.Capacity = Math.Max(entities.Capacity, Count);
            entities.Clear();
            
            var p = EntityKeyListPool.Reserve(registry);
            try
            {
                foreach (var e in p)
                {
                    if (Contains(e))
                    {
                        entities.Add(e);
                    }
                }
            }
            finally
            {
                EntityKeyListPool.Release(p);
            }
        }
        
        public void CopyTo(SparseSet<TEntityKey> entities)
        {
            entities.Capacity = Math.Max(entities.Capacity, Count);
            entities.RemoveAll();
            
            var p = EntityKeyListPool.Reserve(registry);
            try
            {
                foreach (var e in p)
                {
                    if (Contains(e))
                    {
                        entities.Add(e);
                    }
                }
            }
            finally
            {
                EntityKeyListPool.Release(p);
            }
        }

        public struct Enumerator : IEnumerator<TEntityKey>
        {
            readonly NotPool<TEntityKey, TComponent> backend;
            readonly RawList<TEntityKey> contents;
            int index;

            public Enumerator(NotPool<TEntityKey, TComponent> backend)
            {
                this.backend = backend;
                contents = EntityKeyListPool.Reserve(backend.registry);
                Current = default!;
                index = -1;
            }

            public bool MoveNext()
            {
                while (index + 1 < contents.Count)
                {
                    index += 1;
                    if (backend.Contains(contents[index]))
                    {
                        Current = contents[index];
                        return true;
                    }
                }

                Current = default!;
                return false;
            }

            public void Reset()
            {
                index = -1;
                Current = default!;
            }

            object IEnumerator.Current => Current;

            public void Dispose()
            {
                EntityKeyListPool.Release(contents);
            }

            public TEntityKey Current { get; private set; }
        }


    }
}