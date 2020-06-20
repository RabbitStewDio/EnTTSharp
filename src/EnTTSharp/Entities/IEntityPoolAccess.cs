using System;
using System.Collections.Generic;

namespace EnttSharp.Entities
{
    public interface IEntityPoolAccess<TEntityKey> : IEntityViewControl<TEntityKey>,
                                                     IReadOnlyCollection<TEntityKey> 
        where TEntityKey : IEntityKey
    {
        TEntityKey Create();
        Pools.Pool<TEntityKey, TComponent> GetPool<TComponent>();
        event EventHandler<TEntityKey> BeforeEntityDestroyed;
        void AssureEntityState(TEntityKey entity, bool destroyed);
        void Destroy(TEntityKey k);
    }
}