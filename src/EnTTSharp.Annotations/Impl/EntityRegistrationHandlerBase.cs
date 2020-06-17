using System;
using System.Reflection;

namespace EnTTSharp.Annotations.Impl
{
    public abstract class EntityRegistrationHandlerBase : IEntityRegistrationHandler
    {
        static readonly MethodInfo ProcessMethod;

        static EntityRegistrationHandlerBase()
        {
            ProcessMethod = typeof(EntityRegistrationHandlerBase).GetMethod(nameof(ProcessTypedInternal),
                                                                     BindingFlags.NonPublic | BindingFlags.Instance,
                                                                     null,
                                                                     new[] {typeof(EntityComponentRegistration)},
                                                                     null);
            if (ProcessMethod == null)
            {
                throw new InvalidOperationException(" Unable to find private generic method. That really should not happen.");
            }
        }

        public void Process(EntityComponentRegistration reg)
        {
            var typeInfo = reg.TypeInfo;
            var genericMethod = ProcessMethod.MakeGenericMethod(typeInfo);
            genericMethod.Invoke(this, new object[] {reg});
        }

        void ProcessTypedInternal<TComponent>(EntityComponentRegistration r)
        {
            ProcessTyped<TComponent>(r);
        }

        protected abstract void ProcessTyped<TComponent>(EntityComponentRegistration r);
    }
}