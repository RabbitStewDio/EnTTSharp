using System;

namespace EnTTSharp.Entities
{
    public interface IEntityComponentRegistry<TEntityKey>
        where TEntityKey : IEntityKey
    {
        void Register<TComponent>(Func<TComponent> constructorFn,
                                  Action<TEntityKey, IEntityViewControl<TEntityKey>, TComponent> destructorFn = null);

        void RegisterNonConstructable<TComponent>(Action<TEntityKey, IEntityViewControl<TEntityKey>, TComponent> destructorFn = null);
    }
}