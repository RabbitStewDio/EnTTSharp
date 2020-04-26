using EnttSharp.Entities;

namespace EnTTSharp.Annotations
{
    public interface IEntityRegistrationHandler
    {
        void Process(EntityComponentRegistration reg);
    }

    public interface IEntityRegistrationActivator
    {
        void Activate(EntityComponentRegistration r, EntityRegistry registry);
    }
}