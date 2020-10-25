using System;
using System.Reflection;
using EnTTSharp.Entities;
using EnTTSharp.Entities.Attributes;
using Serilog;

namespace EnTTSharp.Annotations.Impl
{
    public class ComponentRegistrationHandler<TEntityKey> : EntityRegistrationHandlerBase where TEntityKey : IEntityKey
    {
        static readonly ILogger Logger = Log.ForContext<ComponentRegistrationHandler<TEntityKey>>();

        protected override void ProcessTyped<TComponent>(EntityComponentRegistration r)
        {
            if (HasDefaultConstructor<TComponent>())
            {
                if (IsFlag<TComponent>())
                {
                    Logger.Debug("Type {ComponentType} has default constructor.", typeof(TComponent));
                    r.WithFlag();
                }
                else
                {
                    Logger.Debug("Type {ComponentType} has default constructor.", typeof(TComponent));
                    r.WithConstructor(Activator.CreateInstance<TComponent>);
                }
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
                    var d = (Action<TEntityKey, IEntityViewControl<TEntityKey>, TComponent>)
                        Delegate.CreateDelegate(typeof(Action<TEntityKey, IEntityViewControl<TEntityKey>, TComponent>), m);
                    r.WithDestructor(d);
                }
            }
        }

        bool IsFlag<TComponent>()
        {
            var componentType = typeof(TComponent);
            var attr = componentType.GetCustomAttribute<EntityComponentAttribute>();
            if (attr == null)
            {
                return false;
            }

            return attr.Constructor == EntityConstructor.Flag;
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
                // Value types that have at least one other constructor are not considered default constructable.
                if (componentType.GetConstructors().Length == 0)
                {
                    Logger.Verbose("Type {ComponentType} is a value type without user constructors.", typeof(TComponent));
                    return true;
                }
                
                Logger.Debug("Type {ComponentType} is a value type with user constructors. This type is considered NonConstructable.", typeof(TComponent));
                return false;
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
                   && methodInfo.IsSameAction(typeof(TEntityKey), 
                                              typeof(IEntityViewControl<TEntityKey>), 
                                              typeof(TComponentType));
        }

    }
}