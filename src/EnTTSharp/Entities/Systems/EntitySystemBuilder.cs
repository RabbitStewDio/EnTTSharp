namespace EnTTSharp.Entities.Systems
{
    public readonly struct EntitySystemBuilder<TEntityKey> where TEntityKey : IEntityKey
    {
        readonly IEntityViewFactory<TEntityKey> reg;
        readonly bool allowParallel;

        public EntitySystemBuilder(IEntityViewFactory<TEntityKey> reg, bool allowParallel)
        {
            this.reg = reg;
            this.allowParallel = allowParallel;
        }

        public EntitySystemBuilder<TEntityKey> AllowParallelExecution()
        {
            return new EntitySystemBuilder<TEntityKey>(reg, true);
        }

        public EntitySystemBuilderWithContext<TEntityKey, TGameContext> WithContext<TGameContext>()
        {
            return new EntitySystemBuilderWithContext<TEntityKey, TGameContext>(reg, EntitySystemExtensions.GlobalAllowParallel && allowParallel);
        }

        public EntitySystemBuilderWithoutContext<TEntityKey> WithoutContext()
        {
            return new EntitySystemBuilderWithoutContext<TEntityKey>(reg, EntitySystemExtensions.GlobalAllowParallel && allowParallel);
        }
    }
}