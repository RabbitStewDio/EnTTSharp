namespace EnTTSharp.Entities.Systems
{
    public readonly partial struct EntitySystemBuilderWithoutContext<TEntityKey> where TEntityKey : IEntityKey
    {
        readonly IEntityViewFactory<TEntityKey> reg;
        readonly bool allowParallel;

        public EntitySystemBuilderWithoutContext(IEntityViewFactory<TEntityKey> reg, bool allowParallel)
        {
            this.reg = reg;
            this.allowParallel = allowParallel;
        }

        public EntitySystemBuilderWithoutContext<TEntityKey> AllowParallelExecution()
        {
            return new EntitySystemBuilderWithoutContext<TEntityKey>(reg, true);
        }

        public EntitySystemBuilder<TEntityKey, TGameContext> WithContext<TGameContext>()
        {
            return new EntitySystemBuilder<TEntityKey, TGameContext>(reg, EntitySystem.GlobalAllowParallel && allowParallel);
        }
    }
}