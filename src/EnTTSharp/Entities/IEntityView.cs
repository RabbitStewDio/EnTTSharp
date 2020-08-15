using System;
using System.Collections.Generic;

namespace EnTTSharp.Entities
{
    /// <summary>
    ///  This is a read-only view over an component pool.
    /// </summary>
    public interface IEntityView<TEntityKey> : IEnumerable<TEntityKey>, IEntityViewControl<TEntityKey>, IDisposable 
        where TEntityKey : IEntityKey
    {
        event EventHandler<TEntityKey> Destroyed;
        event EventHandler<TEntityKey> Created;

        bool AllowParallelExecution { get; set; }

        void Apply(ViewDelegates.Apply<TEntityKey> bulk);
        void ApplyWithContext<TContext>(TContext t, ViewDelegates.ApplyWithContext<TEntityKey, TContext> bulk);

        void Respect<TComponent>();
        void Reserve(int capacity);
    }
}