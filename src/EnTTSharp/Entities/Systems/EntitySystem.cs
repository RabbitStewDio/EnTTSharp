namespace EnTTSharp.Entities.Systems
{
    public static partial class EntitySystem
    {
        public static bool GlobalAllowParallel = true;

        public readonly struct EntitySystemBuilderWithoutContext<TEntityKey> where TEntityKey : IEntityKey
        {
            readonly IEntityViewFactory<TEntityKey> registry;
            readonly bool allowParallel;

            public EntitySystemBuilderWithoutContext(IEntityViewFactory<TEntityKey> registry, bool allowParallel)
            {
                this.registry = registry;
                this.allowParallel = allowParallel;
            }

            public EntitySystemBuilderWithoutContext<TEntityKey> AllowParallelExecution()
            {
                return new EntitySystemBuilderWithoutContext<TEntityKey>(registry, true);
            }

            public EntitySystemBuilder<TEntityKey, TGameContext> WithContext<TGameContext>()
            {
                return new EntitySystemBuilder<TEntityKey, TGameContext>(registry, GlobalAllowParallel && allowParallel);
            }
        }

        public static EntitySystemBuilderWithoutContext<TEntityKey> BuildSystem<TEntityKey>(this IEntityViewFactory<TEntityKey> registry,
                                                                                                      bool allowParallel = false)
            where TEntityKey : IEntityKey
        {
            return new EntitySystemBuilderWithoutContext<TEntityKey>(registry, GlobalAllowParallel && allowParallel);
        }
    }
}