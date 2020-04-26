using System;

namespace EnttSharp.Entities
{
    public static class ComponentRegistration
    {
        class ComponentRegistration0<T> : IComponentRegistration<T>
        {
            readonly EntityRegistry registry;
            readonly Func<T> constructor;
            readonly Action<EntityKey, EntityRegistry, T> destructor;
            public int Index { get; }

            public ComponentRegistration0(int index,
                                          EntityRegistry registry,
                                          Func<T> constructor,
                                          Action<EntityKey, EntityRegistry, T> destructor = null)
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

            public void Destruct(EntityKey k, T o)
            {
                destructor?.Invoke(k, registry, o);
            }

            public bool HasDestructor()
            {
                return destructor != null;
            }
        }

        public static IComponentRegistration<T> Create<T>(int count, EntityRegistry r, Action<EntityKey, EntityRegistry, T> destructor = null)
        {
            return new ComponentRegistration0<T>(count, r,
                                                 () => throw new InvalidOperationException($"The component {typeof(T)} has no registered default constructor."),
                                                 destructor);
        }

        public static IComponentRegistration<T> Create<T>(int count, EntityRegistry r, Func<T> func, Action<EntityKey, EntityRegistry, T> destructor = null)
        {
            return new ComponentRegistration0<T>(count, r, func, destructor);
        }
    }
}