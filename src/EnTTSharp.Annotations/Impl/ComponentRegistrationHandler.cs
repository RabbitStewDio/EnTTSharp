using System;
using System.Reflection;
using EnttSharp.Entities;
using Serilog;

namespace EnTTSharp.Annotations.Impl
{
    public class ComponentRegistrationHandler : EntityRegistrationHandlerBase
    {
        static readonly ILogger Logger = Log.ForContext<ComponentRegistrationHandler>();

        protected override void ProcessTyped<TComponent>(EntityComponentRegistration r)
        {
            if (HasDefaultConstructor<TComponent>())
            {
                Logger.Debug("Type {ComponentType} has default constructor.", typeof(TComponent));
                r.WithConstructor(Activator.CreateInstance<TComponent>);
            }
            else
            {
                r.WithoutConstruction();
            }
            
            var handlerMethods = typeof(TComponent).GetMethods(BindingFlags.Static | BindingFlags.Public);
            foreach (var m in handlerMethods)
            {
                if (IsDestructor<TComponent>(m))
                {
                    var d = (Action<EntityKey, EntityRegistry, TComponent>)
                        Delegate.CreateDelegate(typeof(Action<EntityKey, EntityRegistry, TComponent>), m);
                    r.WithDestructor(d);
                }
            }
        }

        bool HasDefaultConstructor<TComponent>()
        {
            var componentType = typeof(TComponent);
            var attr = componentType.GetCustomAttribute<EntityComponentAttribute>();
            if (attr == null)
            {
                return false;
            }

            if (attr.Constructor == EntityConstructor.NonConstructable)
            {
                Logger.Debug("Type {ComponentType} opted out of using a default constructor.", typeof(TComponent));
                return false;
            }

            if (componentType.IsValueType)
            {
                return true;
            }

            var c = componentType.GetConstructor(Type.EmptyTypes);
            if (c != null)
            {
                return true;
            }

            Logger.Debug("Type {ComponentType} has no default constructor.", typeof(TComponent));
            return false;

        }

        bool IsDestructor<TComponentType>(MethodInfo methodInfo)
        {
            return methodInfo.GetCustomAttribute<EntityDestructorAttribute>() != null 
                   && methodInfo.IsStatic
                   && methodInfo.IsSameAction(typeof(EntityKey), typeof(TComponentType));
        }

    }
}