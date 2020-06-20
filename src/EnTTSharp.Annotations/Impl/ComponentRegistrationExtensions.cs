using System;
using System.Reflection;
using EnttSharp.Entities;

namespace EnTTSharp.Annotations.Impl
{
    public static class ComponentRegistrationExtensions
    {
        class NonConstructibleMarker
        {
        }

        public static EntityComponentRegistration WithConstructor<TComponent>(this EntityComponentRegistration reg, Func<TComponent> constructorFn)
        {
            reg.Store(new ConstructorRegistration<TComponent>(constructorFn));
            return reg;
        }

        public static EntityComponentRegistration WithDestructor<TEntityKey, TComponent>(this EntityComponentRegistration reg,
                                                                                         Action<TEntityKey, IEntityViewControl<TEntityKey>, TComponent> destructorFn) 
            where TEntityKey : IEntityKey
        {
            reg.Store(new DestructorRegistration<TEntityKey, TComponent>(destructorFn));
            return reg;
        }

        public static EntityComponentRegistration WithoutConstruction(this EntityComponentRegistration reg)
        {
            reg.Store(new NonConstructibleMarker());
            return reg;
        }

        public static bool IsSameFunction(this MethodInfo m, Type returnType, params Type[] parameter)
        {
            if (m.ReturnType != returnType)
            {
                return false;
            }

            var p = m.GetParameters();
            if (p.Length != parameter.Length)
            {
                return false;
            }

            for (var i = 0; i < p.Length; i++)
            {
                var pi = p[i];
                if (pi.ParameterType != parameter[i])
                {
                    return false;
                }
            }

            return true;
        }

        public static bool IsSameAction(this MethodInfo m, params Type[] parameter)
        {
            return IsSameFunction(m, typeof(void), parameter);
        }
    }
}