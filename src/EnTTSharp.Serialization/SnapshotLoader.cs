using System;
using EnttSharp.Entities;

namespace EnTTSharp.Serialization
{
    public class SnapshotLoader : ISnapshotLoader
    {
        public SnapshotLoader(EntityRegistry registry)
        {
            this.Registry = registry ?? throw new ArgumentNullException(nameof(registry));
        }

        protected EntityRegistry Registry { get; }

        public void OnEntity(EntityKey entity)
        {
            this.Registry.AssureEntityState(entity, false);
        }

        public void OnDestroyedEntity(EntityKey entity)
        {
            this.Registry.AssureEntityState(entity, true);
        }

        public void OnComponent<TComponent>(EntityKey entity, in TComponent c)
        {
            this.Registry.AssignComponent(entity, c);
        }

        public void OnTag<TComponent>(EntityKey entity, in TComponent c)
        {
            this.Registry.AttachTag(entity, c);
        }

        public void OnTagRemoved<TComponent>()
        {
            this.Registry.RemoveTag<TComponent>();
        }

        public virtual EntityKey Map(EntityKey input)
        {
            return input;
        }

        public void CleanOrphans()
        {
            var p = EntityKeyListPool.Reserve(Registry.GetEnumerator(), Registry.Count);
            try
            {
                foreach (var ek in p)
                {
                    if (Registry.IsOrphan(ek))
                    {
                        Registry.Destroy(ek);
                    }
                }
            }
            finally
            {
                EntityKeyListPool.Release(p);
            }
        }
    }
}