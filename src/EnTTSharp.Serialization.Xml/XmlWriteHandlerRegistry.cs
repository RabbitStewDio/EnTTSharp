using System;
using System.Collections.Generic;
using EnTTSharp.Annotations;
using EnTTSharp.Serialization.Xml.Impl;

namespace EnTTSharp.Serialization.Xml
{
    public class XmlWriteHandlerRegistry
    {
        readonly Dictionary<Type, XmlWriteHandlerRegistration> handlers;

        public XmlWriteHandlerRegistry()
        {
            this.handlers = new Dictionary<Type, XmlWriteHandlerRegistration>();
        }

        public IEnumerable<XmlWriteHandlerRegistration> Handlers => handlers.Values;

        public XmlWriteHandlerRegistry RegisterRange(IEnumerable<EntityComponentRegistration> range)
        {
            foreach (var r in range)
            {
                Register(r);
            }
            return this;
        }

        public XmlWriteHandlerRegistry Register(EntityComponentRegistration reg)
        {
            if (reg.TryGet(out XmlWriteHandlerRegistration r))
            {
                Register(r);
            }
            return this;
        }

        public XmlWriteHandlerRegistry Register(in XmlWriteHandlerRegistration reg)
        {
            handlers.Add(reg.TargetType, reg);
            return this;
        }

        public XmlWriteHandlerRegistry RegisterRange(IEnumerable<XmlWriteHandlerRegistration> range)
        {
            foreach (var r in range)
            {
                Register(r);
            }
            return this;
        }

        public XmlWriteHandlerRegistry Register<TComponent>(string typeId, bool useAsTag = false)
        {
            Register(XmlWriteHandlerRegistration.Create<TComponent>(typeId, new DefaultWriteHandler<TComponent>().Write, useAsTag));
            return this;
        }

        public XmlWriteHandlerRegistration QueryHandler<TComponent>()
        {
            if (handlers.TryGetValue(typeof(TComponent), out var handler))
            {
                return handler;
            }
            throw new ArgumentException("Unable to find write-handler for type " + typeof(TComponent));
        }
    }
}