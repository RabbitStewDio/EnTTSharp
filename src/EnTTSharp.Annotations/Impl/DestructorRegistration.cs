using System;
using EnttSharp.Entities;

namespace EnTTSharp.Annotations.Impl
{
    class DestructorRegistration<TEntityKey, TComponent> where TEntityKey : IEntityKey
    {
        public readonly Action<TEntityKey, IEntityViewControl<TEntityKey>, TComponent> DestructorFn;

        public DestructorRegistration(Action<TEntityKey, IEntityViewControl<TEntityKey>, TComponent> destructorFn)
        {
            this.DestructorFn = destructorFn;
        }
    }
}