using System;
using System.Collections.Generic;
using System.Linq;
using EnttSharp.Entities;

namespace EnTTSharp.Annotations
{
    public class EntityComponentActivator
    {
        readonly List<IEntityRegistrationActivator> activators;

        public EntityComponentActivator(params IEntityRegistrationActivator[] activators)
        {
            this.activators = activators.ToList();
        }

        public EntityComponentActivator With(IEntityRegistrationActivator activator)
        {
            this.activators.Add(activator ?? throw new ArgumentNullException(nameof(activator)));
            return this;
        }

        public void ActivateAll(EntityRegistry registry, IEnumerable<EntityComponentRegistration> components)
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