using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using EnttSharp.Entities;

namespace EnTTSharp.Serialization
{
    public class AsyncSnapshotView 
    {
        readonly EntityRegistry registry;
        readonly List<EntityKey> destroyedEntities;

        public AsyncSnapshotView(EntityRegistry registry)
        {
            this.registry = registry ?? throw new ArgumentNullException(nameof(registry));
            this.destroyedEntities = new List<EntityKey>();

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

        void OnEntityDestroyed(object sender, EntityKey e)
        {
            destroyedEntities.Add(e);
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        public async Task<AsyncSnapshotView> WriteDestroyed(IAsyncEntityArchiveWriter writer)
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

        public async Task<AsyncSnapshotView> WriteEntites(IAsyncEntityArchiveWriter writer)
        {
            await writer.WriteStartEntityAsync(destroyedEntities.Count);
            foreach (var d in registry)
            {
                await writer.WriteEntityAsync(d);
            }
            await writer.WriteEndEntityAsync();

            return this;
        }

        public async Task<AsyncSnapshotView> WriteComponent<TComponent>(IAsyncEntityArchiveWriter writer)
        {
            var pool = registry.GetPool<TComponent>();
            await writer.WriteStartComponentAsync<TComponent>(pool.Count);
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

        public async Task<AsyncSnapshotView> WriteTag<TComponent>(IAsyncEntityArchiveWriter writer)
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