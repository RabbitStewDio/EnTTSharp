using System;

namespace EnttSharp.Entities.Systems
{
    public readonly partial struct EntitySystemBuilder<TEntityKey, TContext> where TEntityKey : IEntityKey
    {
        readonly IEntityViewFactory<TEntityKey> reg;
        readonly bool allowParallel;

        public EntitySystemBuilder(IEntityViewFactory<TEntityKey> registry, bool allowParallelExecution)
        {
            this.allowParallel = allowParallelExecution;
            this.reg = registry;
        }
    }
}