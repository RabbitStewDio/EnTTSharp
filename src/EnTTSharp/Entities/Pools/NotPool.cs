using System;
using System.Collections;
using System.Collections.Generic;
using EnTTSharp.Entities.Helpers;

namespace EnTTSharp.Entities.Pools
{
    public class NotPool<TEntityKey, TComponent>: IReadOnlyPool<TEntityKey, Not<TComponent>> 
        where TEntityKey : IEntityKey
    {
        readonly IEntityPoolAccess<TEntityKey> registry;
        readonly IPool<TEntityKey, TComponent> entityPool;

        public NotPool(IEntityPoolAccess<TEntityKey> registry, 
                       IPool<TEntityKey, TComponent> entityPool)
        {
            this.registry = registry;
            this.entityPool = entityPool ?? throw new ArgumentNullException(nameof(entityPool));
            this.entityPool.Created += HandleCreated;
            this.entityPool.Destroyed += HandleDestroyed;
        }

        void HandleCreated(object sender, TEntityKey e)
        {
            Destroyed?.Invoke(this, e);
        }

        void HandleDestroyed(object sender, TEntityKey e)
        {
            Created?.Invoke(this, e);
        }

        public int Count => registry.Count - entityPool.Count;

        public event EventHandler<TEntityKey> Destroyed;
        public event EventHandler<TEntityKey> Created;

        public bool TryGet(TEntityKey entity, out Not<TComponent> component)
        {
            component = default;
            return Contains(entity);
        }

        public bool Contains(TEntityKey k)
        {
            return registry.IsValid(k) && !entityPool.Contains(k);
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

        public void CopyTo(List<TEntityKey> entites)
        {
            entites.Capacity = Math.Max(entites.Capacity, Count);
            entites.Clear();
            var p = EntityKeyListPool.Reserve(registry);
            foreach (var e in p)
            {
                entites.Add(e);
            }
        }

        public struct Enumerator : IEnumerator<TEntityKey>
        {
            readonly NotPool<TEntityKey, TComponent> backend;
            readonly List<TEntityKey> contents;
            int index;

            public Enumerator(NotPool<TEntityKey, TComponent> backend)
            {
                this.backend = backend;
                contents = EntityKeyListPool.Reserve(backend.registry);
                Current = default;
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

                Current = default;
                return false;
            }

            public void Reset()
            {
                index = -1;
                Current = default;
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