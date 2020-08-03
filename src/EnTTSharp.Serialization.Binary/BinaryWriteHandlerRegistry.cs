using System;
using System.Collections.Generic;
using EnTTSharp.Annotations;

namespace EnTTSharp.Serialization.Binary
{
    public class BinaryWriteHandlerRegistry
    {
        readonly Dictionary<Type, BinaryWriteHandlerRegistration> handlers;

        public BinaryWriteHandlerRegistry()
        {
            this.handlers = new Dictionary<Type, BinaryWriteHandlerRegistration>();
        }

        public IEnumerable<BinaryWriteHandlerRegistration> Handlers => handlers.Values;

        public void RegisterRange(IEnumerable<EntityComponentRegistration> range)
        {
            foreach (var r in range)
            {
                Register(r);
            }
        }

        public void Register(EntityComponentRegistration reg)
        {
            if (reg.TryGet(out BinaryWriteHandlerRegistration r))
            {
                Register(r);
            }
        }

        public void Register(in BinaryWriteHandlerRegistration reg)
        {
            handlers.Add(reg.TargetType, reg);
        }

        public void RegisterRange(IEnumerable<BinaryWriteHandlerRegistration> range)
        {
            foreach (var r in range)
            {
                Register(r);
            }
        }

        public void Register<TComponent>(string typeId, bool useAsTag = false)
        {
            Register(BinaryWriteHandlerRegistration.Create<TComponent>(typeId, useAsTag));
        }

        public BinaryWriteHandlerRegistration QueryHandler<TComponent>()
        {
            if (handlers.TryGetValue(typeof(TComponent), out var handler))
            {
                return handler;
            }
            throw new ArgumentException("Unable to find write-handler for type " + typeof(TComponent));
        }
    }
}