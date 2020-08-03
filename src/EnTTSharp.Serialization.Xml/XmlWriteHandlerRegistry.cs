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

        public void RegisterRange(IEnumerable<EntityComponentRegistration> range)
        {
            foreach (var r in range)
            {
                Register(r);
            }
        }

        public void Register(EntityComponentRegistration reg)
        {
            if (reg.TryGet(out XmlWriteHandlerRegistration r))
            {
                Register(r);
            }
        }

        public void Register(in XmlWriteHandlerRegistration reg)
        {
            handlers.Add(reg.TargetType, reg);
        }

        public void RegisterRange(IEnumerable<XmlWriteHandlerRegistration> range)
        {
            foreach (var r in range)
            {
                Register(r);
            }
        }

        public void Register<TComponent>(string typeId, bool useAsTag = false)
        {
            Register(XmlWriteHandlerRegistration.Create<TComponent>(typeId, new DefaultWriteHandler<TComponent>().Write, useAsTag));
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