using EnTTSharp.Entities;

namespace EnTTSharp.Annotations
{
    public interface IEntityRegistrationHandler
    {
        void Process(EntityComponentRegistration reg);
    }

    public interface IEntityRegistrationActivator<TEntityKey> where TEntityKey : IEntityKey
    {
        void Activate(EntityComponentRegistration r, IEntityComponentRegistry<TEntityKey> registry);
    }
}