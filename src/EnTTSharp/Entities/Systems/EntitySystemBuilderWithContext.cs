using System.Diagnostics.CodeAnalysis;

namespace EnTTSharp.Entities.Systems
{
    [SuppressMessage("ReSharper", "UnusedTypeParameter")]
    public readonly partial struct EntitySystemBuilderWithContext<TEntityKey, TContext> 
        where TEntityKey : IEntityKey
    {
        readonly IEntityViewFactory<TEntityKey> reg;
        readonly bool allowParallel;

        public EntitySystemBuilderWithContext(IEntityViewFactory<TEntityKey> registry, bool allowParallelExecution)
        {
            this.allowParallel = allowParallelExecution;
            this.reg = registry;
        }
    }
    
}