using System;
using System.Reflection;
using EnttSharp.Entities;

namespace EnTTSharp.Annotations.Impl
{
    public abstract class EntityActivatorBase : IEntityRegistrationActivator
    {
        public void Activate(EntityComponentRegistration reg, EntityRegistry registry)
        {
            var typeInfo = reg.TypeInfo;
            var method = typeof(EntityActivatorBase).GetMethod(nameof(ProcessTypedInternal),
                                                               BindingFlags.NonPublic | BindingFlags.Instance,
                                                               null,
                                                               new[] { typeof(EntityComponentRegistration), typeof(EntityRegistry) },
                                                               null);
            if (method == null)
            {
                throw new InvalidOperationException("XUnable to find private generic method. That really should not happen.");
            }

            var genericMethod = method.MakeGenericMethod(typeInfo);
            genericMethod.Invoke(this, new object[] { reg, registry });
        }

        void ProcessTypedInternal<TComponent>(EntityComponentRegistration r, EntityRegistry reg)
        {
            ProcessTyped<TComponent>(r, reg);
        }

        protected abstract void ProcessTyped<TComponent>(EntityComponentRegistration r, EntityRegistry reg);
    }
}