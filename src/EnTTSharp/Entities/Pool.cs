using System;
using System.Collections.Generic;
using EnttSharp.Entities.Helpers;

namespace EnttSharp.Entities
{
    public interface ISparsePool<TEntityKey> : IEnumerable<TEntityKey>
        where TEntityKey: IEntityKey
    {
        event EventHandler<TEntityKey> Destroyed;
        event EventHandler<TEntityKey> Created;
        bool Contains(TEntityKey k);
        int Count { get; }
        void Reserve(int capacity);
        new RawList<TEntityKey>.Enumerator GetEnumerator();
    }

    public static class Pools
    {
        public class Pool<TEntityKey, TComponent> : SparseDictionary<TEntityKey, TComponent>, ISparsePool<TEntityKey>
            where TEntityKey : IEntityKey
        {
            public event EventHandler<(TEntityKey key, TComponent old)> DestroyedNotify;
            public event EventHandler<TEntityKey> Destroyed;
            public event EventHandler<TEntityKey> Created;
            public event EventHandler<(TEntityKey key, TComponent old)> Updated;

            internal Pool()
            {
            }

            public override void Add(TEntityKey e, in TComponent component)
            {
                base.Add(e, in component);
                Created?.Invoke(this, e);
            }

            public override bool Remove(TEntityKey e)
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

            public override bool WriteBack(TEntityKey entity, in TComponent component)
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

        public class DestructorPool<TEntityKey, TComponent> : Pool<TEntityKey, TComponent>
            where TEntityKey : IEntityKey
        {
            readonly IComponentRegistration<TEntityKey, TComponent> componentRegistration;

            internal DestructorPool(IComponentRegistration<TEntityKey, TComponent> componentRegistration)
            {
                this.componentRegistration = componentRegistration ??
                                             throw new ArgumentNullException(nameof(componentRegistration));
            }

            public override bool Remove(TEntityKey e)
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

            public override bool Replace(TEntityKey entity, in TComponent component)
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

        public static Pool<TEntityKey, T> Create<TEntityKey, T>(IComponentRegistration<TEntityKey, T> reg)
            where TEntityKey : IEntityKey
        {
            if (reg.HasDestructor())
            {
                return new DestructorPool<TEntityKey, T>(reg);
            }

            return new Pool<TEntityKey, T>();
        }
    }
}