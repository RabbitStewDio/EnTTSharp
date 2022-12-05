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
            var pool = Registry.GetPool<TComponent>();
            pool.Updated += OnPositionUpdated;
            pool.Created += OnPositionCreated;
            pool.DestroyedNotify += OnDestroyed;
        }

        public void Dispose()
        {
            Dispose(true);
        }

        protected virtual void Dispose(bool disposing)
        {
            var pool = Registry.GetPool<TComponent>();
            pool.Updated -= OnPositionUpdated;
            pool.Created -= OnPositionCreated;
            pool.DestroyedNotify -= OnDestroyed;
        }

        void OnDestroyed(object sender, (TEntityKey k, TComponent old) x) => OnPositionDestroyed(sender, (x.k, x.old));
        
        protected virtual void OnPositionDestroyed(object sender, (TEntityKey key, Optional<TComponent> old) e) { }
        protected abstract void OnPositionUpdated(object sender, (TEntityKey key, TComponent c) e);
        protected abstract void OnPositionCreated(object sender, (TEntityKey key, TComponent c) e);
    }
}