﻿using EnTTSharp.Entities;

namespace EnTTSharp.Entities
{
    public static partial class ViewDelegates
    {
        public delegate void Apply<TEntityKey>(IEntityViewControl<TEntityKey> v, TEntityKey k) 
            where TEntityKey : IEntityKey;
        public delegate void ApplyWithContext<TEntityKey, in TContext>(IEntityViewControl<TEntityKey> v, TContext context, TEntityKey k)
            where TEntityKey : IEntityKey;

    }
}
