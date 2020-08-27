using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using EnTTSharp.Entities;
using EnTTSharp.Entities.Helpers;

namespace EnTTSharp.Serialization
{
    public class AsyncSnapshotView<TEntityKey> where TEntityKey : IEntityKey
    {
        readonly IEntityPoolAccess<TEntityKey> registry;
        readonly List<TEntityKey> destroyedEntities;

        public AsyncSnapshotView(IEntityPoolAccess<TEntityKey> registry)
        {
            this.registry = registry ?? throw new ArgumentNullException(nameof(registry));
            this.destroyedEntities = new List<TEntityKey>();

            this.registry.BeforeEntityDestroyed += OnEntityDestroyed;
        }

        ~AsyncSnapshotView()
        {
            Dispose(false);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                this.registry.BeforeEntityDestroyed -= OnEntityDestroyed;
            }
        }

        void OnEntityDestroyed(object sender, TEntityKey e)
        {
            destroyedEntities.Add(e);
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        public async Task<AsyncSnapshotView<TEntityKey>> WriteDestroyed(IAsyncEntityArchiveWriter<TEntityKey> writer)
        {
            await writer.WriteStartDestroyedAsync(destroyedEntities.Count);
            foreach (var d in destroyedEntities)
            {
                await writer.WriteDestroyedAsync(d);
            }
            await writer.WriteEndDestroyedAsync();

            destroyedEntities.Clear();
            return this;
        }

        public async Task<AsyncSnapshotView<TEntityKey>> WriteEntites(IAsyncEntityArchiveWriter<TEntityKey> writer)
        {
            await writer.WriteStartEntityAsync(destroyedEntities.Count);
            var p = EntityKeyListPool<TEntityKey>.Reserve(registry);
            foreach (var d in p)
            {
                await writer.WriteEntityAsync(d);
            }
            await writer.WriteEndEntityAsync();

            return this;
        }

        public async Task<AsyncSnapshotView<TEntityKey>> WriteComponent<TComponent>(IAsyncEntityArchiveWriter<TEntityKey> writer)
        {
            var pool = registry.GetPool<TComponent>();
            await writer.WriteStartComponentAsync<TComponent>(pool.Count);
            var p = EntityKeyListPool.Reserve(pool);
            foreach (var entity in pool)
            {
                if (pool.TryGet(entity, out var c))
                {
                    await writer.WriteComponentAsync(entity, c);
                }
            }
            await writer.WriteEndComponentAsync<TComponent>();

            return this;
        }

        public async Task<AsyncSnapshotView<TEntityKey>> WriteTag<TComponent>(IAsyncEntityArchiveWriter<TEntityKey> writer)
        {
            if (registry.TryGetTag(out var entity, out TComponent tag))
            {
                await writer.WriteTagAsync(entity, tag);
            }
            else
            {
                await writer.WriteMissingTagAsync<TComponent>();
            }

            return this;
        }
    }
}