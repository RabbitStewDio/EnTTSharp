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
        new RawList<TEntityKey>.Enumerator GetEnumerator();
    }
}