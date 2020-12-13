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
    }
    
    
}