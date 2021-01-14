using System;
using System.Collections.Generic;
using EnTTSharp.Entities.Pools;

namespace EnTTSharp.Entities
{
    public interface IEntityPoolAccess<TEntityKey> : IEntityViewControl<TEntityKey>,
                                                     IReadOnlyCollection<TEntityKey> 
        where TEntityKey : IEntityKey
    {
        TEntityKey Create();
        IPool<TEntityKey, TComponent> GetWritablePool<TComponent>();
        IReadOnlyPool<TEntityKey, TComponent> GetPool<TComponent>();
        bool TryGetPool<TComponent>(out IReadOnlyPool<TEntityKey, TComponent> pool);
        bool TryGetWritablePool<TComponent>(out IPool<TEntityKey, TComponent> pool);
        event EventHandler<TEntityKey> BeforeEntityDestroyed;
        void AssureEntityState(TEntityKey entity, bool destroyed);
        void Destroy(TEntityKey k);

        void CopyTo(List<TEntityKey> k);
    }
}