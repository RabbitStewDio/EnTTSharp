using System.Collections.Generic;

namespace EnTTSharp.Entities.Pools
{
    public interface IPool<TEntityKey, TComponent>: IReadOnlyPool<TEntityKey, TComponent>, 
                                                    IPool<TEntityKey>
        where TEntityKey : IEntityKey
    {
        void Add(TEntityKey e, in TComponent component);
        bool WriteBack(TEntityKey entity, in TComponent component);
        ref TComponent? TryGetModifiableRef(TEntityKey entity, ref TComponent? defaultValue, out bool success);
    }

    public interface IPool<TEntityKey> : IReadOnlyPool<TEntityKey>
        where TEntityKey : IEntityKey
    {
        void Respect(IEnumerable<TEntityKey> other);
        bool Remove(TEntityKey e);
        void RemoveAll();
    }
}