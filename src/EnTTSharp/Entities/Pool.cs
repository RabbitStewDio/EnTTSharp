using System;
using System.Collections.Generic;
using EnttSharp.Entities.Helpers;

namespace EnttSharp.Entities
{
    public interface ISparsePool : IEnumerable<EntityKey>
    {
        event EventHandler<EntityKey> Destroyed;
        event EventHandler<EntityKey> Created;
        bool Contains(EntityKey k);
        int Count { get; }
        void Reserve(int capacity);
        new RawList<EntityKey>.Enumerator GetEnumerator();
    }

    public static class Pools
    {
        public class Pool<TComponent> : SparseDictionary<TComponent>, ISparsePool
        {
            public event EventHandler<(EntityKey key, TComponent old)> DestroyedNotify;
            public event EventHandler<EntityKey> Destroyed;
            public event EventHandler<EntityKey> Created;
            public event EventHandler<(EntityKey key, TComponent old)> Updated;

            internal Pool()
            {
            }

            public override void Add(EntityKey e, in TComponent component)
            {
                base.Add(e, in component);
                Created?.Invoke(this, e);
            }

            public override bool Remove(EntityKey e)
            {
                if (Destroyed == null && DestroyedNotify == null)
                {
                    return base.Remove(e);
                }

                if (DestroyedNotify == null)
                {
                    if (base.Remove(e))
                    {
                        Destroyed?.Invoke(this, e);
                        return true;
                    }
                }
                else if (TryGet(e, out var old) && base.Remove(e))
                {
                    DestroyedNotify?.Invoke(this, (e, old));
                    Destroyed?.Invoke(this, e);
                    return true;
                }

                return false;
            }

            public override void RemoveAll()
            {
                if (Destroyed == null && DestroyedNotify == null)
                {
                    base.RemoveAll();
                    return;
                }

                while (Count > 0)
                {
                    var k = Last;
                    Remove(k);
                }
            }

            public override bool WriteBack(EntityKey entity, in TComponent component)
            {
                if (Updated == null)
                {
                    return base.WriteBack(entity, in component);
                }

                if (TryGet(entity, out var c))
                {
                    if (base.WriteBack(entity, in component))
                    {
                        Updated?.Invoke(this, (entity, c));
                        return true;
                    }
                }

                return false;
            }
        }

        public class DestructorPool<TComponent> : Pool<TComponent>
        {
            readonly IComponentRegistration<TComponent> componentRegistration;

            internal DestructorPool(IComponentRegistration<TComponent> componentRegistration)
            {
                this.componentRegistration = componentRegistration ??
                                             throw new ArgumentNullException(nameof(componentRegistration));
            }

            public override bool Remove(EntityKey e)
            {
                if (TryGet(e, out var com))
                {
                    if (base.Remove(e))
                    {
                        componentRegistration.Destruct(e, com);
                        return true;
                    }
                }

                return false;
            }

            public override bool Replace(EntityKey entity, in TComponent component)
            {
                if (TryGet(entity, out var c))
                {
                    var retval = base.Replace(entity, in component);
                    if (retval)
                    {
                        componentRegistration.Destruct(entity, c);
                    }

                    return retval;
                }

                return false;
            }
        }

        public static Pool<T> Create<T>(IComponentRegistration<T> reg)
        {
            if (reg.HasDestructor())
            {
                return new DestructorPool<T>(reg);
            }

            return new Pool<T>();
        }
    }
}