using System;

namespace EnTTSharp.Annotations.Impl
{
    class ConstructorRegistration<TComponent>
    {
        public readonly Func<TComponent> ConstructorFn;

        public ConstructorRegistration(Func<TComponent> constructorFn)
        {
            this.ConstructorFn = constructorFn;
        }
    }
}