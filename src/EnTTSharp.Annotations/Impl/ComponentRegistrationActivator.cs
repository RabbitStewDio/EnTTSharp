using EnttSharp.Entities;

namespace EnTTSharp.Annotations.Impl
{
    public class ComponentRegistrationActivator : EntityActivatorBase
    {
        protected override void ProcessTyped<TComponent>(EntityComponentRegistration r, EntityRegistry reg)
        {
            bool hasConstructor = r.TryGet(out ConstructorRegistration<TComponent> constructor);
            bool hasDestructor = r.TryGet(out DestructorRegistration<TComponent> destructor);

            if (hasConstructor && hasDestructor)
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