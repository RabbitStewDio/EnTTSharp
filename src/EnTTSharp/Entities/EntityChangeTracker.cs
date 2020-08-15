using System;

namespace EnTTSharp.Entities
{
    public abstract class EntityChangeTracker<TEntityKey, TEntity>: IDisposable 
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
            var pool = Registry.GetPool<TEntity>();
            pool.Updated += OnPositionUpdated;
            pool.Created += OnPositionCreated;
            pool.DestroyedNotify += OnPositionDestroyed;
        }

        public void Dispose()
        {
            Dispose(true);
        }

        protected virtual void Dispose(bool disposing)
        {
            var pool = Registry.GetPool<TEntity>();
            pool.Updated -= OnPositionUpdated;
            pool.Created -= OnPositionCreated;
            pool.DestroyedNotify -= OnPositionDestroyed;
        }

        protected virtual void OnPositionDestroyed(object sender, (TEntityKey key, TEntity old) e) { }
        protected abstract void OnPositionUpdated(object sender, (TEntityKey key, TEntity old) e);
        protected abstract void OnPositionCreated(object sender, TEntityKey e);
    }
}