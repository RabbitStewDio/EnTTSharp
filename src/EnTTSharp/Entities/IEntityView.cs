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

    public interface IEntityView<T1> : IEntityView
    {
        void Apply(ViewDelegates.Apply<T1> bulk);
        void ApplyWithContext<TContext>(TContext t, ViewDelegates.ApplyWithContext<TContext, T1> bulk);
    }

    public interface IEntityView<T1, T2> : IEntityView
    {
        void Apply(ViewDelegates.Apply<T1, T2> bulk);
        void ApplyWithContext<TContext>(TContext t, ViewDelegates.ApplyWithContext<TContext, T1, T2> bulk);
    }

    public interface IEntityView<T1, T2, T3> : IEntityView
    {
        void Apply(ViewDelegates.Apply<T1, T2, T3> bulk);
        void ApplyWithContext<TContext>(TContext t, ViewDelegates.ApplyWithContext<TContext, T1, T2, T3> bulk);
    }

    public interface IEntityView<T1, T2, T3, T4> : IEntityView
    {
        void Apply(ViewDelegates.Apply<T1, T2, T3, T4> bulk);
        void ApplyWithContext<TContext>(TContext t, ViewDelegates.ApplyWithContext<TContext, T1, T2, T3, T4> bulk);
    }

    public interface IEntityView<T1, T2, T3, T4, T5> : IEntityView
    {
        void Apply(ViewDelegates.Apply<T1, T2, T3, T4, T5> bulk);
        void ApplyWithContext<TContext>(TContext t, ViewDelegates.ApplyWithContext<TContext, T1, T2, T3, T4, T5> bulk);
    }

    public interface IEntityView<T1, T2, T3, T4, T5, T6> : IEntityView
    {
        void Apply(ViewDelegates.Apply<T1, T2, T3, T4, T5, T6> bulk);
        void ApplyWithContext<TContext>(TContext t, ViewDelegates.ApplyWithContext<TContext, T1, T2, T3, T4, T5, T6> bulk);
    }

    public interface IEntityView<T1, T2, T3, T4, T5, T6, T7> : IEntityView
    {
        void Apply(ViewDelegates.Apply<T1, T2, T3, T4, T5, T6, T7> bulk);
        void ApplyWithContext<TContext>(TContext t, ViewDelegates.ApplyWithContext<TContext, T1, T2, T3, T4, T5, T6, T7> bulk);
    }
}