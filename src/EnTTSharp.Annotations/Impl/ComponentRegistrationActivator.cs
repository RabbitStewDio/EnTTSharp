using System;
using EnTTSharp.Entities;

namespace EnTTSharp.Annotations.Impl
{
    public class ComponentRegistrationActivator<TEntityKey> : EntityActivatorBase<TEntityKey> where TEntityKey : IEntityKey
    {
        static readonly Lazy<ComponentRegistrationActivator<TEntityKey>> InstanceHolder = new Lazy<ComponentRegistrationActivator<TEntityKey>>();
        public static ComponentRegistrationActivator<TEntityKey> Instance => InstanceHolder.Value; 

        protected override void ProcessTyped<TComponent>(EntityComponentRegistration r, IEntityComponentRegistry<TEntityKey> reg)
        {
            bool hasConstructor = r.TryGet(out ConstructorRegistration<TComponent> constructor);
            bool hasDestructor = r.TryGet(out DestructorRegistration<TEntityKey, TComponent> destructor);

            if (r.TryGet(out ComponentRegistrationExtensions.FlagMarker m))
            {
                reg.RegisterFlag<TComponent>();
            }
            else if (hasConstructor && hasDestructor)
            {
                reg.Register(constructor.ConstructorFn, destructor.DestructorFn);
            }
            else if (hasConstructor)
            {
                reg.Register(constructor.ConstructorFn);
            }
            else if (hasDestructor)
            {
                reg.RegisterNonConstructable(destructor.DestructorFn);
            }
            else
            {
                reg.RegisterNonConstructable<TComponent>();
            }
        }
    }
}