using System;

namespace EnttSharp.Entities.Systems
{
    public readonly partial struct EntitySystemBuilder<TContext>
    {
        readonly EntityRegistry reg;
        readonly bool allowParallel;

        public EntitySystemBuilder(EntityRegistry registry, bool allowParallelExecution)
        {
            this.allowParallel = allowParallelExecution;
            this.reg = registry;
        }
    }
}