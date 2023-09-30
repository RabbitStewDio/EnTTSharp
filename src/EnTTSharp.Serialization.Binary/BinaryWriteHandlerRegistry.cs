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

        public BinaryWriteHandlerRegistry RegisterRange(IEnumerable<EntityComponentRegistration> range)
        {
            foreach (var r in range)
            {
                Register(r);
            }

            return this;
        }

        public BinaryWriteHandlerRegistry Register(EntityComponentRegistration reg)
        {
            if (reg.TryGet(out BinaryWriteHandlerRegistration r))
            {
                Register(r);
            }

            return this;
        }

        public BinaryWriteHandlerRegistry Register(in BinaryWriteHandlerRegistration reg)
        {
            handlers.Add(reg.TargetType, reg);
            return this;
        }

        public BinaryWriteHandlerRegistry RegisterRange(IEnumerable<BinaryWriteHandlerRegistration> range)
        {
            foreach (var r in range)
            {
                Register(r);
            }
            return this;
        }

        public BinaryWriteHandlerRegistry Register<TComponent>(string typeId, bool useAsTag = false)
        {
            Register(BinaryWriteHandlerRegistration.Create<TComponent>(typeId, useAsTag));
            return this;
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