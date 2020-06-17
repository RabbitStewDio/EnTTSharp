using System;
using System.Collections.Generic;
using EnttSharp.Entities;

namespace EnTTSharp.Serialization
{
    public class SnapshotView 
    {
        readonly EntityRegistry registry;
        readonly List<EntityKey> destroyedEntities;

        public SnapshotView(EntityRegistry registry)
        {
            this.registry = registry ?? throw new ArgumentNullException(nameof(registry));
            this.destroyedEntities = new List<EntityKey>();

            this.registry.BeforeEntityDestroyed += OnEntityDestroyed;
        }

        ~SnapshotView()
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

        public SnapshotView WriteEndOfFrame(IEntityArchiveWriter writer, bool forceFlush)
        {
            writer.WriteEndOfFrame();
            if (forceFlush)
            {
                writer.FlushFrame();
            }

            return this;
        }

        public SnapshotView WriteDestroyed(IEntityArchiveWriter writer)
        {
            writer.WriteStartDestroyed(destroyedEntities.Count);
            foreach (var d in destroyedEntities)
            {
                writer.WriteDestroyed(d);
            }
            writer.WriteEndDestroyed();

            destroyedEntities.Clear();
            return this;
        }

        public SnapshotView WriteEntites(IEntityArchiveWriter writer)
        {
            writer.WriteStartEntity(registry.Count);
            foreach (var d in registry)
            {
                writer.WriteEntity(d);
            }
            writer.WriteEndEntity();

            return this;
        }

        public SnapshotView WriteComponent<TComponent>(IEntityArchiveWriter writer)
        {
            var pool = registry.GetPool<TComponent>();
            writer.WriteStartComponent<TComponent>(pool.Count);
            foreach (var entity in pool)
            {
                if (pool.TryGet(entity, out var c))
                {
                    writer.WriteComponent(entity, c);
                }
            }
            writer.WriteEndComponent<TComponent>();

            return this;
        }

        public SnapshotView WriteTag<TComponent>(IEntityArchiveWriter writer)
        {
            if (registry.TryGetTag(out var entity, out TComponent tag))
            {
                writer.WriteTag(entity, tag);
            }
            else
            {
                writer.WriteMissingTag<TComponent>();
            }

            return this;
        }
    }
}