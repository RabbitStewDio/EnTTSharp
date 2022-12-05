using System;
using System.Collections.Generic;
using EnTTSharp.Entities.Helpers;
using System.Diagnostics.CodeAnalysis;

namespace EnTTSharp.Entities.Pools
{
    public interface IReadOnlyPool<TEntityKey, TComponent> : IReadOnlyPool<TEntityKey>
        where TEntityKey : IEntityKey
    {
        event EventHandler<(TEntityKey key, TComponent old)>? DestroyedNotify;
        event EventHandler<(TEntityKey key, TComponent old)>? Updated;
        event EventHandler<(TEntityKey key, TComponent old)>? Created;
        
        bool TryGet(TEntityKey entity, [MaybeNullWhen(false)] out TComponent component);
        ref readonly TComponent? TryGetRef(TEntityKey entity, ref TComponent? defaultValue, out bool success);
    }

    public interface IReadOnlyPool<TEntityKey> : IEnumerable<TEntityKey>
        where TEntityKey : IEntityKey
    {
        event EventHandler<TEntityKey> Destroyed;
        event EventHandler<TEntityKey> CreatedEntry;
        event EventHandler<TEntityKey> UpdatedEntry;

        bool Contains(TEntityKey k);
        int Count { get; }
        void Reserve(int capacity);

        void CopyTo(RawList<TEntityKey> entites);
    }
}