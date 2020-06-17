using System;
using System.Reflection;
using EnttSharp.Entities;

namespace EnTTSharp.Annotations.Impl
{
    public abstract class EntityActivatorBase : IEntityRegistrationActivator
    {
        static readonly MethodInfo ProcessMethodInfo;

        static EntityActivatorBase()
        {
            var method = typeof(EntityActivatorBase).GetMethod(nameof(ProcessTypedInternal),
                                                               BindingFlags.NonPublic | BindingFlags.Instance,
                                                               null,
                                                               new[] { typeof(EntityComponentRegistration), typeof(EntityRegistry) },
                                                               null);
            if (method == null)
            {
                throw new InvalidOperationException("Unable to find private generic method. That really should not happen.");
            }

            ProcessMethodInfo = method;
        }

        public void Activate(EntityComponentRegistration reg, EntityRegistry registry)
        {
            var typeInfo = reg.TypeInfo;
            var genericMethod = ProcessMethodInfo.MakeGenericMethod(typeInfo);
            genericMethod.Invoke(this, new object[] { reg, registry });
        }

        void ProcessTypedInternal<TComponent>(EntityComponentRegistration r, EntityRegistry reg)
        {
            ProcessTyped<TComponent>(r, reg);
        }

        protected abstract void ProcessTyped<TComponent>(EntityComponentRegistration r, EntityRegistry reg);
    }
}