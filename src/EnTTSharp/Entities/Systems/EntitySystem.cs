namespace EnTTSharp.Entities.Systems
{
    public static partial class EntitySystem
    {
        public static bool GlobalAllowParallel = true;


        public static EntitySystemBuilderWithoutContext<TEntityKey> BuildSystem<TEntityKey>(this IEntityViewFactory<TEntityKey> registry,
                                                                                                      bool allowParallel = false)
            where TEntityKey : IEntityKey
        {
            return new EntitySystemBuilderWithoutContext<TEntityKey>(registry, GlobalAllowParallel && allowParallel);
        }
    }
}