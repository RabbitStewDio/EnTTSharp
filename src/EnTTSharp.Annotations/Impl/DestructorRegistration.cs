using System;
using EnttSharp.Entities;

namespace EnTTSharp.Annotations.Impl
{
    class DestructorRegistration<TComponent>
    {
        public readonly Action<EntityKey, EntityRegistry, TComponent> DestructorFn;

        public DestructorRegistration(Action<EntityKey, EntityRegistry, TComponent> constructorFn)
        {
            this.DestructorFn = constructorFn;
        }
    }
}