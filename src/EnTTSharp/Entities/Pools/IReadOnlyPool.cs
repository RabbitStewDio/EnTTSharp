using System;
using System.Collections.Generic;
using EnTTSharp.Entities.Helpers;

namespace EnTTSharp.Entities.Pools
{
    public interface IReadOnlyPool<TEntityKey> : IEnumerable<TEntityKey>
        where TEntityKey : IEntityKey
    {
        event EventHandler<TEntityKey> Destroyed;
        event EventHandler<TEntityKey> Created;

        bool Contains(TEntityKey k);
        int Count { get; }
        void Reserve(int capacity);

        void CopyTo(List<TEntityKey> entites);
        void CopyTo(SparseSet<TEntityKey> entites);
    }
}