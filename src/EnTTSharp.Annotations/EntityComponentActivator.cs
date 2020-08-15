using System;
using System.Collections.Generic;
using System.Linq;
using EnttSharp.Entities;
using EnTTSharp.Entities;

namespace EnTTSharp.Annotations
{
    public class EntityComponentActivator<TEntityKey> where TEntityKey : IEntityKey
    {
        readonly List<IEntityRegistrationActivator<TEntityKey>> activators;

        public EntityComponentActivator(params IEntityRegistrationActivator<TEntityKey>[] activators)
        {
            this.activators = activators.ToList();
        }

        public EntityComponentActivator<TEntityKey> With(IEntityRegistrationActivator<TEntityKey> activator)
        {
            this.activators.Add(activator ?? throw new ArgumentNullException(nameof(activator)));
            return this;
        }

        public void ActivateAll(IEntityComponentRegistry<TEntityKey> registry,
                                IEnumerable<EntityComponentRegistration> components) 
        {
            registry = registry ?? throw new ArgumentNullException(nameof(registry));
            foreach (var c in components)
            {
                foreach (var a in activators)
                {
                    a.Activate(c, registry);
                }
            }
        }
    }
}