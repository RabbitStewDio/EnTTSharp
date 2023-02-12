using EnTTSharp.Entities.Helpers;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace EnTTSharp.Entities.Pools;

public class DerivedValuePool<TEntityKey, TSourceComponent, TComponent>: IReadOnlyPool<TEntityKey, TComponent> 
    where TEntityKey : IEntityKey
{
    readonly IReadOnlyPool<TEntityKey, TSourceComponent> parentPool;
    readonly Func<TEntityKey, TSourceComponent, TComponent> derivateFunction;

    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

    public DerivedValuePool(IReadOnlyPool<TEntityKey, TSourceComponent> parentPool, 
                            Func<TEntityKey, TSourceComponent, TComponent> derivateFunction)
    {
        this.parentPool = parentPool;
        this.derivateFunction = derivateFunction;
        this.parentPool.Destroyed += OnDestroyed;
        this.parentPool.CreatedEntry += OnCreatedEntity;
        this.parentPool.UpdatedEntry += OnUpdatedEntity;
        this.parentPool.DestroyedNotify += OnDestroyedNotify;
        this.parentPool.Created += OnCreated;
        this.parentPool.Updated += OnUpdated;
    }

    void OnCreated(object sender, (TEntityKey key, TSourceComponent old) e)
    {
        Created?.Invoke(this, (e.key, derivateFunction(e.key, e.old)));
    }

    void OnUpdated(object sender, (TEntityKey key, TSourceComponent old) e)
    {
        Updated?.Invoke(this, (e.key, derivateFunction(e.key, e.old)));
    }

    void OnDestroyedNotify(object sender, (TEntityKey key, TSourceComponent old) e)
    {
        DestroyedNotify?.Invoke(this, (e.key, derivateFunction(e.key, e.old)));
    }

    void OnCreatedEntity(object sender, TEntityKey e)
    {
        CreatedEntry?.Invoke(this, e);
    }

    void OnUpdatedEntity(object sender, TEntityKey e)
    {
        UpdatedEntry?.Invoke(this, e);
    }

    void OnDestroyed(object sender, TEntityKey e)
    {
        Destroyed?.Invoke(this, e);
    }

    public IEnumerator<TEntityKey> GetEnumerator()
    {
        return parentPool.GetEnumerator();
    }

    public event EventHandler<TEntityKey>? Destroyed;
    public event EventHandler<TEntityKey>? CreatedEntry;
    public event EventHandler<TEntityKey>? UpdatedEntry;

    public bool Contains(TEntityKey k) => parentPool.Contains(k);

    public int Count => parentPool.Count;
    
    public void Reserve(int capacity)
    {
        parentPool.Reserve(capacity);
    }

    public void CopyTo(RawList<TEntityKey> entities)
    {
        parentPool.CopyTo(entities);
    }

    public event EventHandler<(TEntityKey key, TComponent old)>? DestroyedNotify;
    public event EventHandler<(TEntityKey key, TComponent old)>? Updated;
    public event EventHandler<(TEntityKey key, TComponent old)>? Created;

    public bool TryGet(TEntityKey entity, [MaybeNullWhen(false)] out TComponent component)
    {
        if (parentPool.TryGet(entity, out var c))
        {
            component = derivateFunction(entity, c);
            return true;
        }

        component = default;
        return false;
    }

    public ref readonly TComponent? TryGetRef(TEntityKey entity, ref TComponent? defaultValue, out bool success)
    {
        if (parentPool.TryGet(entity, out var c))
        {
            defaultValue = derivateFunction(entity, c);
            success = true;
            return ref defaultValue;
        }

        success = false;
        return ref defaultValue;
    }
}

