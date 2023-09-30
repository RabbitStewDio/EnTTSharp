using System;
using System.Collections.Generic;
using EnTTSharp.Entities;

namespace EnTTSharp.Serialization
{
    public class SnapshotView<TEntityKey> : IDisposable where TEntityKey : IEntityKey
    {
        readonly IEntityPoolAccess<TEntityKey> registry;
        readonly List<TEntityKey> destroyedEntities;

        public SnapshotView(IEntityPoolAccess<TEntityKey> registry)
        {
            this.registry = registry ?? throw new ArgumentNullException(nameof(registry));
            this.destroyedEntities = new List<TEntityKey>();

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

        void OnEntityDestroyed(object sender, TEntityKey e)
        {
            destroyedEntities.Add(e);
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        public SnapshotView<TEntityKey> WriteEndOfFrame(IEntityArchiveWriter<TEntityKey> writer, bool forceFlush)
        {
            writer.WriteEndOfFrame();
            if (forceFlush)
            {
                writer.FlushFrame();
            }

            return this;
        }

        public SnapshotView<TEntityKey> WriteDestroyed(IEntityArchiveWriter<TEntityKey> writer)
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

        public SnapshotView<TEntityKey> WriteEntites(IEntityArchiveWriter<TEntityKey> writer)
        {
            writer.WriteStartEntity(registry.Count);
            foreach (var d in registry)
            {
                writer.WriteEntity(d);
            }

            writer.WriteEndEntity();

            return this;
        }

        public SnapshotView<TEntityKey> WriteComponent<TComponent>(IEntityArchiveWriter<TEntityKey> writer)
        {
            try
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
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }

            return this;
        }

        public SnapshotView<TEntityKey> WriteTag<TComponent>(IEntityArchiveWriter<TEntityKey> writer)
        {
            if (registry.TryGetTag<TComponent>(out var entity, out var tag))
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