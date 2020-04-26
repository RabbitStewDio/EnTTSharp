using System.Collections.Generic;
using EnttSharp.Entities;

namespace EnTTSharp.Serialization
{
    public class ContinuousSnapshotLoader : SnapshotLoader
    {
        readonly Dictionary<EntityKey, EntityKey> remoteMapping;

        public ContinuousSnapshotLoader(EntityRegistry registry) : base(registry)
        {
            remoteMapping = new Dictionary<EntityKey, EntityKey>();
        }

        public override EntityKey Map(EntityKey input)
        {
            if (remoteMapping.TryGetValue(input, out var mapped))
            {
                return mapped;
            }

            var local = Registry.Create();
            remoteMapping[input] = local;
            return local;
        }
    }
}