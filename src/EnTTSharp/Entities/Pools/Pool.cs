using System;
using EnTTSharp.Entities.Helpers;

namespace EnTTSharp.Entities.Pools
{
    public interface IReadOnlyPool<TEntityKey, TComponent> : IReadOnlyPool<TEntityKey>
        where TEntityKey : IEntityKey
    {
        bool TryGet(TEntityKey entity, out TComponent component);
    }

    public interface IPool<TEntityKey, TComponent>: IReadOnlyPool<TEntityKey, TComponent>, 
                                                    IPool<TEntityKey>
        where TEntityKey : IEntityKey
    {
        event EventHandler<(TEntityKey key, TComponent old)> DestroyedNotify;
        event EventHandler<(TEntityKey key, TComponent old)> Updated;

        void Add(TEntityKey e, in TComponent component);
        bool WriteBack(TEntityKey entity, in TComponent component);
    }

    public class Pool<TEntityKey, TComponent> : SparseDictionary<TEntityKey, TComponent>, IPool<TEntityKey, TComponent>
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
}