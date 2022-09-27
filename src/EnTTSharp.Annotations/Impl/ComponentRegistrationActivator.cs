using System;
using EnTTSharp.Entities;

namespace EnTTSharp.Annotations.Impl
{
    public class ComponentRegistrationActivator<TEntityKey> : EntityActivatorBase<TEntityKey> where TEntityKey : IEntityKey
    {
        static readonly Lazy<ComponentRegistrationActivator<TEntityKey>> instanceHolder = new Lazy<ComponentRegistrationActivator<TEntityKey>>();
        public static ComponentRegistrationActivator<TEntityKey> Instance => instanceHolder.Value; 

        protected override void ProcessTyped<TComponent>(EntityComponentRegistration r, IEntityComponentRegistry<TEntityKey> reg)
        {
            bool hasConstructor = r.TryGet<ConstructorRegistration<TComponent>>(out var constructor);
            bool hasDestructor = r.TryGet<DestructorRegistration<TEntityKey, TComponent>>(out var destructor);

            if (r.TryGet(out ComponentRegistrationExtensions.FlagMarker _))
            {
                reg.RegisterFlag<TComponent>();
            }
            else if (hasConstructor && hasDestructor)
            {
                reg.Register(constructor!.ConstructorFn, destructor!.DestructorFn);
            }
            else if (hasConstructor)
            {
                reg.Register(constructor!.ConstructorFn);
            }
            else if (hasDestructor)
            {
                reg.RegisterNonConstructable(destructor!.DestructorFn);
            }
            else
            {
                reg.RegisterNonConstructable<TComponent>();
            }
        }
    }
}