using System;

namespace EnTTSharp.Entities
{
    public abstract class EntityChangeTracker<TEntityKey, TComponent>: IDisposable 
        where TEntityKey : IEntityKey
    {
        protected readonly EntityRegistry<TEntityKey> Registry;

        protected EntityChangeTracker(EntityRegistry<TEntityKey> registry)
        {
            this.Registry = registry;
        }

        ~EntityChangeTracker()
        {
            Dispose(false);
        }

        public void Install()
        {
            if (Registry.TryGetWritablePool<TComponent>(out var pool))
            {
                pool.Updated += OnPositionUpdated;
                pool.Created += OnPositionCreated;
                pool.DestroyedNotify += OnPositionDestroyed;
            }
            else
            {
                var roPool = Registry.GetPool<TComponent>();
                roPool.Created += OnPositionCreated;
                roPool.Destroyed += OnBasicDestroyed;
            }
        }

        public void Dispose()
        {
            Dispose(true);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (Registry.TryGetWritablePool<TComponent>(out var pool))
            {
                pool.Updated -= OnPositionUpdated;
                pool.Created -= OnPositionCreated;
                pool.DestroyedNotify -= OnPositionDestroyed;
            }
            else
            {
                var roPool = Registry.GetPool<TComponent>();
                roPool.Created -= OnPositionCreated;
                roPool.Destroyed -= OnBasicDestroyed;
            }
        }

        void OnBasicDestroyed(object sender, TEntityKey e)
        {
            OnPositionDestroyed(sender, (e, default));
        }

        protected virtual void OnPositionDestroyed(object sender, (TEntityKey key, TComponent old) e) { }
        protected abstract void OnPositionUpdated(object sender, (TEntityKey key, TComponent old) e);
        protected abstract void OnPositionCreated(object sender, TEntityKey e);
    }
}