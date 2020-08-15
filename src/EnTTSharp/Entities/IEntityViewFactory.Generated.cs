
//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace EnTTSharp.Entities
{
    public interface IEntityViewFactory<TEntityKey> : IEntityViewControl<TEntityKey>
        where TEntityKey: IEntityKey
    {
        void DiscardView<TView>() where TView : IEntityView<TEntityKey>;

        IEntityView<TEntityKey, T1> View<T1>();
        IPersistentEntityView<TEntityKey, T1> PersistentView<T1>();

        IEntityView<TEntityKey, T1, T2> View<T1, T2>();
        IPersistentEntityView<TEntityKey, T1, T2> PersistentView<T1, T2>();

        IEntityView<TEntityKey, T1, T2, T3> View<T1, T2, T3>();
        IPersistentEntityView<TEntityKey, T1, T2, T3> PersistentView<T1, T2, T3>();

        IEntityView<TEntityKey, T1, T2, T3, T4> View<T1, T2, T3, T4>();
        IPersistentEntityView<TEntityKey, T1, T2, T3, T4> PersistentView<T1, T2, T3, T4>();

        IEntityView<TEntityKey, T1, T2, T3, T4, T5> View<T1, T2, T3, T4, T5>();
        IPersistentEntityView<TEntityKey, T1, T2, T3, T4, T5> PersistentView<T1, T2, T3, T4, T5>();

        IEntityView<TEntityKey, T1, T2, T3, T4, T5, T6> View<T1, T2, T3, T4, T5, T6>();
        IPersistentEntityView<TEntityKey, T1, T2, T3, T4, T5, T6> PersistentView<T1, T2, T3, T4, T5, T6>();

        IEntityView<TEntityKey, T1, T2, T3, T4, T5, T6, T7> View<T1, T2, T3, T4, T5, T6, T7>();
        IPersistentEntityView<TEntityKey, T1, T2, T3, T4, T5, T6, T7> PersistentView<T1, T2, T3, T4, T5, T6, T7>();

  }

}