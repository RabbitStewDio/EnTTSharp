using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using EnTTSharp.Entities.Helpers;

namespace EnTTSharp.Entities.Pools
{
    public class FlagPool<TEntityKey, TData> : IPool<TEntityKey, TData>
        where TEntityKey : IEntityKey
    {
        readonly TData sharedData;
        readonly SparseSet<TEntityKey> backend;

        public FlagPool(TData sharedData)
        {
            this.sharedData = sharedData;
            backend = new SparseSet<TEntityKey>();
        }

        public bool TryGet(TEntityKey entity, out TData component)
        {
            component = sharedData;
            return backend.Contains(entity);
        }

        public void Add(TEntityKey e, in TData component)
        {
            backend.Add(e);
            CreatedEntry?.Invoke(this, e);
            Created?.Invoke(this, (e, sharedData));
        }

        public virtual bool WriteBack(TEntityKey entity, in TData component)
        {
            return backend.Contains(entity);
        }

        public event EventHandler<(TEntityKey key, TData old)>? DestroyedNotify;
        public event EventHandler<TEntityKey>? Destroyed;
        public event EventHandler<TEntityKey>? CreatedEntry;
        public event EventHandler<(TEntityKey key, TData old)>? Created;

        public int Count => backend.Count;

        public bool Contains(TEntityKey k) => backend.Contains(k);

        protected TEntityKey Last
        {
            get { return backend.Last; }
        }

        public void Respect(IEnumerable<TEntityKey> other)
        {
            backend.Respect(other);
        }

        public void Reserve(int capacity)
        {
            backend.Reserve(capacity);
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        IEnumerator<TEntityKey> IEnumerable<TEntityKey>.GetEnumerator()
        {
            return GetEnumerator();
        }

        public SegmentedRawList<TEntityKey>.Enumerator GetEnumerator()
        {
            return backend.GetEnumerator();
        }

        public event EventHandler<(TEntityKey key, TData old)>? Updated
        {
            add { }
            remove { }
        }

        public event EventHandler<TEntityKey>? UpdatedEntry
        {
            add { }
            remove { }
        }


        public virtual bool Remove(TEntityKey e)
        {
            if (Destroyed == null && DestroyedNotify == null)
            {
                return backend.Remove(e);
            }

            if (DestroyedNotify == null)
            {
                if (backend.Remove(e))
                {
                    Destroyed?.Invoke(this, e);
                    return true;
                }
            }
            else if (TryGet(e, out var old) && backend.Remove(e))
            {
                DestroyedNotify?.Invoke(this, (e, old));
                Destroyed?.Invoke(this, e);
                return true;
            }

            return false;
        }

        public virtual void RemoveAll()
        {
            if (Destroyed == null && DestroyedNotify == null)
            {
                backend.RemoveAll();
                return;
            }

            while (backend.Count > 0)
            {
                var k = backend.Last;
                Remove(k);
            }
        }

        [SuppressMessage("ReSharper", "RedundantAssignment")]
        public ref readonly TData? TryGetRef(TEntityKey entity, ref TData? defaultValue, out bool success)
        {
            if (Contains(entity))
            {
                defaultValue = sharedData;
                success = true;
            }
            else
            {
                success = false;
            }
            return ref defaultValue;
        }

        public ref TData? TryGetModifiableRef(TEntityKey entity, ref TData? defaultValue, out bool success)
        {
            if (Contains(entity))
            {
                defaultValue = sharedData;
                success = true;
            }
            else
            {
                success = false;
            }
            return ref defaultValue;
        }

        public void CopyTo(RawList<TEntityKey> entites)
        {
            entites.Capacity = Math.Max(entites.Capacity, Count);
            entites.Clear();
            backend.CopyTo(entites);
        }
    }
}