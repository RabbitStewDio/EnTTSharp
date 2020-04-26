using System;

namespace EnttSharp.Entities
{
    public static class EntitySystem
    {
        const bool globalAllowParallel = false;

        public static Action<TContext>
            CreateSystem<TContext, TTrait>(EntityRegistry reg,
                                           ViewDelegates.ApplyWithContext<TContext, TTrait> action, bool allowParallel = false)
        {
            var view = reg.PersistentView<TTrait>();
            view.AllowParallelExecution = globalAllowParallel && allowParallel;

            void Act(TContext context)
            {
                view.ApplyWithContext(context, action);
            }

            return Act;
        }

        public static Action<TContext>
            CreateSystem<TContext, TTraitA, TTraitB>(
                EntityRegistry reg,
                ViewDelegates.ApplyWithContext<TContext, TTraitA, TTraitB> action, bool allowParallel = false)
        {
            var view = reg.PersistentView<TTraitA, TTraitB>();
            view.AllowParallelExecution = globalAllowParallel && allowParallel;

            void Act(TContext context)
            {
                view.ApplyWithContext(context, action);
            }

            return Act;
        }

        public static Action<TContext> CreateSystem<TContext, TTraitA, TTraitB, TTraitC>(
            EntityRegistry reg, ViewDelegates.ApplyWithContext<TContext, TTraitA, TTraitB, TTraitC> action, bool allowParallel = false)
        {
            var view = reg.PersistentView<TTraitA, TTraitB, TTraitC>();
            view.AllowParallelExecution = globalAllowParallel && allowParallel;

            void Act(TContext context)
            {
                view.ApplyWithContext(context, action);
            }

            return Act;
        }

        public static Action<TContext> CreateSystem<TContext, TTraitA, TTraitB, TTraitC, TTraitD>(
            EntityRegistry reg, ViewDelegates.ApplyWithContext<TContext, TTraitA, TTraitB, TTraitC, TTraitD> action, bool allowParallel = false)
        {
            var view = reg.PersistentView<TTraitA, TTraitB, TTraitC, TTraitD>();
            view.AllowParallelExecution = globalAllowParallel && allowParallel;

            void Act(TContext context)
            {
                view.ApplyWithContext(context, action);
            }

            return Act;
        }

        public static Action<TContext> CreateSystem<TContext, TTraitA, TTraitB, TTraitC, TTraitD, TraitE>(
            EntityRegistry reg, ViewDelegates.ApplyWithContext<TContext, TTraitA, TTraitB, TTraitC, TTraitD, TraitE> action, bool allowParallel = false)
        {
            var view = reg.PersistentView<TTraitA, TTraitB, TTraitC, TTraitD, TraitE>();
            view.AllowParallelExecution = globalAllowParallel && allowParallel;

            void Act(TContext context)
            {
                view.ApplyWithContext(context, action);
            }

            return Act;
        }

        public static Action<TContext> CreateSystem<TContext, TTraitA, TTraitB, TTraitC, TTraitD, TraitE, TraitF>(
            EntityRegistry reg, ViewDelegates.ApplyWithContext<TContext, TTraitA, TTraitB, TTraitC, TTraitD, TraitE, TraitF> action, bool allowParallel = false)
        {
            var view = reg.PersistentView<TTraitA, TTraitB, TTraitC, TTraitD, TraitE, TraitF>();
            view.AllowParallelExecution = globalAllowParallel && allowParallel;

            void Act(TContext context)
            {
                view.ApplyWithContext(context, action);
            }

            return Act;
        }
    }
}