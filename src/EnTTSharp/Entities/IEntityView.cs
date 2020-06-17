using System;
using System.Collections.Generic;

namespace EnttSharp.Entities
{
    /// <summary>
    ///  This is a read-only view over an component pool.
    /// </summary>
    public interface IEntityView : IEnumerable<EntityKey>, IEntityViewControl, IDisposable
    {
        event EventHandler<EntityKey> Destroyed;
        event EventHandler<EntityKey> Created;

        bool AllowParallelExecution { get; set; }

        void Apply(ViewDelegates.Apply bulk);
        void ApplyWithContext<TContext>(TContext t, ViewDelegates.ApplyWithContext<TContext> bulk);

        void Respect<TComponent>();
        void Reserve(int capacity);
    }
}