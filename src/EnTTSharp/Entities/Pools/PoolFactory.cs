namespace EnTTSharp.Entities.Pools
{
    public static class PoolFactory
    {
        public static IPool<TEntityKey, T> Create<TEntityKey, T>(IComponentRegistration<TEntityKey, T> reg)
            where TEntityKey : IEntityKey
        {
            if (reg.HasDestructor())
            {
                return new DestructorPool<TEntityKey, T>(reg);
            }

            return new Pool<TEntityKey, T>();
        }

        public static IPool<TEntityKey, T> CreateFlagPool<TEntityKey, T>(T sharedData, IComponentRegistration<TEntityKey, T> reg)
            where TEntityKey : IEntityKey
        {
            if (reg.HasDestructor())
            {
                return new DestructorFlagPool<TEntityKey, T>(sharedData, reg);
            }

            return new FlagPool<TEntityKey, T>(sharedData);
        }
    }
}