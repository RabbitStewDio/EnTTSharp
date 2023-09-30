using System;
using System.Collections.Generic;
using EnTTSharp.Entities;
using EnTTSharp.Entities.Helpers;

namespace EnTTSharp.Serialization
{
    public class SnapshotLoader<TEntityKey> : ISnapshotLoader<TEntityKey>, IDisposable 
        where TEntityKey : IEntityKey
    {
        readonly Dictionary<EntityKeyData, TEntityKey> remoteMapping;
        readonly Dictionary<TEntityKey, EntityKeyData> localMapping;

        public SnapshotLoader(IEntityPoolAccess<TEntityKey> registry)
        {
            this.Registry = registry ?? throw new ArgumentNullException(nameof(registry));
            registry.BeforeEntityDestroyed += OnLocalEntityDestroyed;
            remoteMapping = new Dictionary<EntityKeyData, TEntityKey>();
            localMapping = new Dictionary<TEntityKey, EntityKeyData>();
        }

        protected IEntityPoolAccess<TEntityKey> Registry { get; }

        public void OnEntity(TEntityKey entity)
        {
            this.Registry.AssureEntityState(entity, false);
        }

        public void OnDestroyedEntity(TEntityKey entity)
        {
            this.Registry.AssureEntityState(entity, true);
        }

        public void OnComponent<TComponent>(TEntityKey entity, in TComponent c)
        {
            this.Registry.AssignComponent(entity, c);
        }

        public void OnTag<TComponent>(TEntityKey entity, in TComponent c)
        {
            this.Registry.AttachTag(entity, c);
        }

        public void OnTagRemoved<TComponent>()
        {
            this.Registry.RemoveTag<TComponent>();
        }

        public TEntityKey Map(TEntityKey input)
        {
            return Map(new EntityKeyData(input.Age, input.Key));
        }

        public void CleanOrphans()
        {
            var p = EntityKeyListPool<TEntityKey>.Reserve(Registry);
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
                EntityKeyListPool<TEntityKey>.Release(p);
            }
        }

        ~SnapshotLoader()
        {
            Registry.BeforeEntityDestroyed -= OnLocalEntityDestroyed;
        }

        void OnLocalEntityDestroyed(object sender, TEntityKey e)
        {
            if (localMapping.TryGetValue(e, out var remoteKey))
            {
                remoteMapping.Remove(remoteKey);
                localMapping.Remove(e);
            }
        }

        public virtual TEntityKey Map(EntityKeyData input)
        {
            if (remoteMapping.TryGetValue(input, out var mapped))
            {
                return mapped;
            }

            var local = Registry.Create();
            remoteMapping[input] = local;
            localMapping[local] = input;
            return local;
        }

        public bool TryLookupMapping(TEntityKey input, out TEntityKey mapped)
        {
            return TryLookupMapping(new EntityKeyData(input.Age, input.Key), out mapped);
        }

        public bool TryLookupMapping(EntityKeyData input, out TEntityKey mapped)
        {
            return remoteMapping.TryGetValue(input, out mapped);
        }

        public void Dispose()
        {
            Registry.BeforeEntityDestroyed -= OnLocalEntityDestroyed;
            GC.SuppressFinalize(this);
        }
    }
}