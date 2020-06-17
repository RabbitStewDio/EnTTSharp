namespace EnttSharp.Entities.Systems
{
    public static partial class EntitySystem
    {
        public static bool GlobalAllowParallel = true;

        public static EntitySystemBuilder<TContext> BuildSystem<TContext>(this EntityRegistry registry, bool allowParallel = true)
        {
            return new EntitySystemBuilder<TContext>(registry, GlobalAllowParallel && allowParallel);
        }
/*
        [Obsolete]
        public static Action<TContext>
            CreateSystem<TContext, TTrait>(EntityRegistry reg,
                                           ViewDelegates.ApplyWithContext<TContext, TTrait> action, bool allowParallel = false)
        {
            return reg.BuildSystem<TContext>(allowParallel).CreateSystem(action);
        }

        [Obsolete]
        public static Action<TContext>
            CreateSystem<TContext, TTraitA, TTraitB>(
                EntityRegistry reg,
                ViewDelegates.ApplyWithContext<TContext, TTraitA, TTraitB> action, bool allowParallel = false)
        {
            return reg.BuildSystem<TContext>(allowParallel).CreateSystem(action);
        }

        [Obsolete]
        public static Action<TContext> CreateSystem<TContext, TTraitA, TTraitB, TTraitC>(
            EntityRegistry reg, ViewDelegates.ApplyWithContext<TContext, TTraitA, TTraitB, TTraitC> action, bool allowParallel = false)
        {
            var view = reg.PersistentView<TTraitA, TTraitB, TTraitC>();
            view.AllowParallelExecution = GlobalAllowParallel && allowParallel;

            void Act(TContext context)
            {
                view.ApplyWithContext(context, action);
            }

            return Act;
        }

        [Obsolete]
        public static Action<TContext> CreateSystem<TContext, TTraitA, TTraitB, TTraitC, TTraitD>(
            EntityRegistry reg, ViewDelegates.ApplyWithContext<TContext, TTraitA, TTraitB, TTraitC, TTraitD> action, bool allowParallel = false)
        {
            var view = reg.PersistentView<TTraitA, TTraitB, TTraitC, TTraitD>();
            view.AllowParallelExecution = GlobalAllowParallel && allowParallel;

            void Act(TContext context)
            {
                view.ApplyWithContext(context, action);
            }

            return Act;
        }

        [Obsolete]
        public static Action<TContext> CreateSystem<TContext, TTraitA, TTraitB, TTraitC, TTraitD, TTraitE>(
            EntityRegistry reg, ViewDelegates.ApplyWithContext<TContext, TTraitA, TTraitB, TTraitC, TTraitD, TTraitE> action, bool allowParallel = false)
        {
            var view = reg.PersistentView<TTraitA, TTraitB, TTraitC, TTraitD, TTraitE>();
            view.AllowParallelExecution = GlobalAllowParallel && allowParallel;

            void Act(TContext context)
            {
                view.ApplyWithContext(context, action);
            }

            return Act;
        }

        [Obsolete]
        public static Action<TContext> CreateSystem<TContext, TTraitA, TTraitB, TTraitC, TTraitD, TTraitE, TTraitF>(
            EntityRegistry reg, ViewDelegates.ApplyWithContext<TContext, TTraitA, TTraitB, TTraitC, TTraitD, TTraitE, TTraitF> action, bool allowParallel = false)
        {
            var view = reg.PersistentView<TTraitA, TTraitB, TTraitC, TTraitD, TTraitE, TTraitF>();
            view.AllowParallelExecution = GlobalAllowParallel && allowParallel;

            void Act(TContext context)
            {
                view.ApplyWithContext(context, action);
            }

            return Act;
        }
        */
    }
}