﻿namespace EnttSharp.Entities
{
    public interface IEntityViewFactory: IEntityViewControl
    {
        IEntityView<T1> View<T1>();

        IEntityView<T1, T2> View<T1, T2>();
        IEntityView<T1, T2, T3> View<T1, T2, T3>();
        IEntityView<T1, T2, T3, T4> View<T1, T2, T3, T4>();

        IEntityView<T1, T2, T3, T4, T5> View<T1, T2, T3, T4, T5>();
        IEntityView<T1, T2, T3, T4, T5, T6> View<T1, T2, T3, T4, T5, T6>();

        IEntityView<T1, T2, T3, T4, T5, T6, T7> View<T1, T2, T3, T4, T5, T6, T7>();

        void DiscardView<TView>() where TView : IEntityView;

        IPersistentEntityView<T1> PersistentView<T1>();

        IPersistentEntityView<T1, T2> PersistentView<T1, T2>();

        IPersistentEntityView<T1, T2, T3> PersistentView<T1, T2, T3>();

        IPersistentEntityView<T1, T2, T3, T4> PersistentView<T1, T2, T3, T4>();

        IPersistentEntityView<T1, T2, T3, T4, T5> PersistentView<T1, T2, T3, T4, T5>();

        IPersistentEntityView<T1, T2, T3, T4, T5, T6> PersistentView<T1, T2, T3, T4, T5, T6>();

        IPersistentEntityView<T1, T2, T3, T4, T5, T6, T7> PersistentView<T1, T2, T3, T4, T5, T6, T7>();
    }
}