using System;

namespace EnTTSharp.Entities
{
    public static class ComponentRegistration
    {
        class ComponentRegistration0<TEntityKey, T> : IComponentRegistration<TEntityKey, T> 
            where TEntityKey : IEntityKey
        {
            readonly EntityRegistry<TEntityKey> registry;
            readonly Func<T> constructor;
            readonly Action<TEntityKey, EntityRegistry<TEntityKey>, T>? destructor;
            public int Index { get; }

            public ComponentRegistration0(int index,
                                          EntityRegistry<TEntityKey> registry,
                                          Func<T> constructor,
                                          Action<TEntityKey, EntityRegistry<TEntityKey>, T>? destructor = null)
            {
                this.registry = registry;
                this.constructor = constructor;
                this.destructor = destructor;
                Index = index;
            }

            public T Create()
            {
                return constructor();
            }

            public void Destruct(TEntityKey k, T o)
            {
                destructor?.Invoke(k, registry, o);
            }

            public bool HasDestructor()
            {
                return destructor != null;
            }
        }

        public static IComponentRegistration<TEntityKey, T> Create<TEntityKey, T>(int count, 
                                                                                  EntityRegistry<TEntityKey> r, 
                                                                                  Action<TEntityKey, EntityRegistry<TEntityKey>, T>? destructor = null) 
            where TEntityKey : IEntityKey
        {
            return new ComponentRegistration0<TEntityKey, T>(count, r,
                                                             () => throw new InvalidOperationException($"The component {typeof(T)} has no registered default constructor."),
                                                             destructor);
        }

        public static IComponentRegistration<TEntityKey, T> Create<TEntityKey, T>(int count, 
                                                                                            EntityRegistry<TEntityKey> r, 
                                                                                            Func<T> func, 
                                                                                            Action<TEntityKey, EntityRegistry<TEntityKey>, T>? destructor = null)
            where TEntityKey : IEntityKey
        {
            return new ComponentRegistration0<TEntityKey, T>(count, r, func, destructor);
        }
    }
}