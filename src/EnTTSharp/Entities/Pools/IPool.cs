using System.Collections.Generic;

namespace EnTTSharp.Entities.Pools
{
    public interface IPool<TEntityKey> : IReadOnlyPool<TEntityKey> 
        where TEntityKey : IEntityKey
    {
        void Respect(IEnumerable<TEntityKey> other);
        bool Remove(TEntityKey e);
        void RemoveAll();
    }
}