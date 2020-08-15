using System;

//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------
 
namespace EnTTSharp.Entities.Systems
{
    public static partial class EntitySystem
    {

        [Obsolete]
        public static Action<TContext>
            CreateSystem<TEntityKey, TContext, T1 >(IEntityViewFactory<TEntityKey> reg,
                                           ViewDelegates.ApplyWithContext<TEntityKey, TContext, T1> action, bool allowParallel = false)
            where TEntityKey: IEntityKey
        {
            return reg.BuildSystem<TEntityKey, TContext>(allowParallel).CreateSystem(action);
        }

        [Obsolete]
        public static Action<TContext>
            CreateSystem<TEntityKey, TContext, T1, T2 >(IEntityViewFactory<TEntityKey> reg,
                                           ViewDelegates.ApplyWithContext<TEntityKey, TContext, T1, T2> action, bool allowParallel = false)
            where TEntityKey: IEntityKey
        {
            return reg.BuildSystem<TEntityKey, TContext>(allowParallel).CreateSystem(action);
        }

        [Obsolete]
        public static Action<TContext>
            CreateSystem<TEntityKey, TContext, T1, T2, T3 >(IEntityViewFactory<TEntityKey> reg,
                                           ViewDelegates.ApplyWithContext<TEntityKey, TContext, T1, T2, T3> action, bool allowParallel = false)
            where TEntityKey: IEntityKey
        {
            return reg.BuildSystem<TEntityKey, TContext>(allowParallel).CreateSystem(action);
        }

        [Obsolete]
        public static Action<TContext>
            CreateSystem<TEntityKey, TContext, T1, T2, T3, T4 >(IEntityViewFactory<TEntityKey> reg,
                                           ViewDelegates.ApplyWithContext<TEntityKey, TContext, T1, T2, T3, T4> action, bool allowParallel = false)
            where TEntityKey: IEntityKey
        {
            return reg.BuildSystem<TEntityKey, TContext>(allowParallel).CreateSystem(action);
        }

        [Obsolete]
        public static Action<TContext>
            CreateSystem<TEntityKey, TContext, T1, T2, T3, T4, T5 >(IEntityViewFactory<TEntityKey> reg,
                                           ViewDelegates.ApplyWithContext<TEntityKey, TContext, T1, T2, T3, T4, T5> action, bool allowParallel = false)
            where TEntityKey: IEntityKey
        {
            return reg.BuildSystem<TEntityKey, TContext>(allowParallel).CreateSystem(action);
        }

        [Obsolete]
        public static Action<TContext>
            CreateSystem<TEntityKey, TContext, T1, T2, T3, T4, T5, T6 >(IEntityViewFactory<TEntityKey> reg,
                                           ViewDelegates.ApplyWithContext<TEntityKey, TContext, T1, T2, T3, T4, T5, T6> action, bool allowParallel = false)
            where TEntityKey: IEntityKey
        {
            return reg.BuildSystem<TEntityKey, TContext>(allowParallel).CreateSystem(action);
        }

        [Obsolete]
        public static Action<TContext>
            CreateSystem<TEntityKey, TContext, T1, T2, T3, T4, T5, T6, T7 >(IEntityViewFactory<TEntityKey> reg,
                                           ViewDelegates.ApplyWithContext<TEntityKey, TContext, T1, T2, T3, T4, T5, T6, T7> action, bool allowParallel = false)
            where TEntityKey: IEntityKey
        {
            return reg.BuildSystem<TEntityKey, TContext>(allowParallel).CreateSystem(action);
        }

    }
}