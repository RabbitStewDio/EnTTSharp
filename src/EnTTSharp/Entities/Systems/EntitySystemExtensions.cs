namespace EnTTSharp.Entities.Systems
{
    public static class EntitySystemExtensions
    {
        public static bool GlobalAllowParallel = true;

        public static EntitySystemBuilder<TEntityKey> BuildSystem<TEntityKey>(this IEntityViewFactory<TEntityKey> registry, bool allowParallel = false)
            where TEntityKey : IEntityKey
        {
            return new EntitySystemBuilder<TEntityKey>(registry, GlobalAllowParallel && allowParallel);
        }
    }
}