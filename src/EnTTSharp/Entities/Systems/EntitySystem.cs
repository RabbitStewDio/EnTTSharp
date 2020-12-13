namespace EnTTSharp.Entities.Systems
{
    public static partial class EntitySystem
    {
        public static bool GlobalAllowParallel = true;

        public static EntitySystemBuilder<TEntityKey> BuildSystem<TEntityKey>(this IEntityViewFactory<TEntityKey> registry,
                                                                                            bool allowParallel = false)
            where TEntityKey : IEntityKey
        {
            return new EntitySystemBuilder<TEntityKey>(registry, GlobalAllowParallel && allowParallel);
        }
    }
}