using System;
using System.Collections.Generic;
using EnttSharp.Entities;

namespace EnTTSharp.Serialization
{
    public class ContinuousSnapshotLoader : SnapshotLoader, IDisposable
    {
        readonly EntityRegistry registry;
        readonly Dictionary<EntityKey, EntityKey> remoteMapping;
        readonly Dictionary<EntityKey, EntityKey> localMapping;

        public ContinuousSnapshotLoader(EntityRegistry registry) : base(registry)
        {
            this.registry = registry;
            registry.BeforeEntityDestroyed += OnLocalEntityDestroyed;
            remoteMapping = new Dictionary<EntityKey, EntityKey>();
            localMapping = new Dictionary<EntityKey, EntityKey>();
        }

        ~ContinuousSnapshotLoader()
        {
            registry.BeforeEntityDestroyed -= OnLocalEntityDestroyed;
        }

        void OnLocalEntityDestroyed(object sender, EntityKey e)
        {
            if (localMapping.TryGetValue(e, out var remoteKey))
            {
                remoteMapping.Remove(remoteKey);
                localMapping.Remove(e);
            }
        }

        public override EntityKey Map(EntityKey input)
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

        public void Dispose()
        {
            registry.BeforeEntityDestroyed -= OnLocalEntityDestroyed;
            GC.SuppressFinalize(this);
        }
    }
}