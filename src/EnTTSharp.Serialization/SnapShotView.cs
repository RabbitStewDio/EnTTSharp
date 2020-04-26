using System;
using System.Collections.Generic;
using EnttSharp.Entities;

namespace EnTTSharp.Serialization
{
    public class SnapShotView : ISnapShotView
    {
        readonly EntityRegistry registry;
        readonly List<EntityKey> destroyedEntities;

        public SnapShotView(EntityRegistry registry)
        {
            this.registry = registry ?? throw new ArgumentNullException(nameof(registry));
            this.destroyedEntities = new List<EntityKey>();

            this.registry.BeforeEntityDestroyed += OnEntityDestroyed;
        }

        ~SnapShotView()
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

        public ISnapShotView WriteDestroyed(IEntityArchiveWriter writer)
        {
            foreach (var d in destroyedEntities)
            {
                writer.WriteDestroyed(d);
            }

            destroyedEntities.Clear();
            return this;
        }

        public ISnapShotView WriteEntites(IEntityArchiveWriter writer)
        {
            foreach (var d in registry)
            {
                writer.WriteEntity(d);
            }

            return this;
        }

        public ISnapShotView WriteComponent<TComponent>(IEntityArchiveWriter writer)
        {
            var pool = registry.GetPool<TComponent>();
            foreach (var entity in pool)
            {
                if (pool.TryGet(entity, out var c))
                {
                    writer.WriteComponent(entity, c);
                }
            }

            return this;
        }

        public ISnapShotView WriteTag<TComponent>(IEntityArchiveWriter writer)
        {
            if (registry.TryGetTag(out var entity, out TComponent tag))
            {
                writer.WriteTag(entity, tag);
            }

            return this;
        }
    }

    public static class SnapShotExtensions
    {
        public static ISnapShotView CreateSnapshot(this EntityRegistry r)
        {
            return new SnapShotView(r);
        }
    }
}