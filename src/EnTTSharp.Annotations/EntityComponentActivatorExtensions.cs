using System.Collections.Generic;
using EnTTSharp.Annotations.Impl;
using EnTTSharp.Entities;

namespace EnTTSharp.Annotations
{
    public static class EntityComponentActivatorExtensions
    {
        public static EntityRegistry<TEntity> Register<TEntity>(this EntityRegistry<TEntity> registry,
                                                                EntityComponentRegistration componentRegistration)
            where TEntity : IEntityKey
        {
            ComponentRegistrationActivator<TEntity>.Instance.Activate(componentRegistration, registry);
            return registry;
        }

        public static EntityRegistry<TEntity> RegisterAll<TEntity>(this EntityRegistry<TEntity> registry,
                                                                   IEnumerable<EntityComponentRegistration> componentRegistration)
            where TEntity : IEntityKey
        {
            foreach (var c in componentRegistration)
            {
                ComponentRegistrationActivator<TEntity>.Instance.Activate(c, registry);
            }

            return registry;
        }
    }
}