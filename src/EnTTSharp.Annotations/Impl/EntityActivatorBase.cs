using System;
using System.Reflection;
using EnttSharp.Entities;

namespace EnTTSharp.Annotations.Impl
{
    public abstract class EntityActivatorBase<TEntityKey> : IEntityRegistrationActivator<TEntityKey> 
        where TEntityKey : IEntityKey
    {
        static readonly MethodInfo ProcessMethodInfo;

        static EntityActivatorBase()
        {
            var method = typeof(EntityActivatorBase<TEntityKey>).GetMethod(nameof(ProcessTypedInternal),
                                                               BindingFlags.NonPublic | BindingFlags.Instance,
                                                               null,
                                                               new[] { typeof(EntityComponentRegistration), typeof(IEntityComponentRegistry<TEntityKey>) },
                                                               null);
            if (method == null)
            {
                throw new InvalidOperationException("Unable to find private generic method. That really should not happen.");
            }

            ProcessMethodInfo = method;
        }

        public void Activate(EntityComponentRegistration reg, IEntityComponentRegistry<TEntityKey> registry)
        {
            var typeInfo = reg.TypeInfo;
            var genericMethod = ProcessMethodInfo.MakeGenericMethod(typeInfo);
            genericMethod.Invoke(this, new object[] { reg, registry });
        }

        void ProcessTypedInternal<TComponent>(EntityComponentRegistration r, IEntityComponentRegistry<TEntityKey> reg)
        {
            ProcessTyped<TComponent>(r, reg);
        }

        protected abstract void ProcessTyped<TComponent>(EntityComponentRegistration r, IEntityComponentRegistry<TEntityKey> reg);
    }
}