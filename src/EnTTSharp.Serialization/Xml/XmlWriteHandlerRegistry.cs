using System;
using System.Collections.Generic;

namespace EnTTSharp.Serialization.Xml
{
    public class XmlWriteHandlerRegistry
    {
        readonly Dictionary<Type, XmlWriteHandlerRegistration> handlers;

        public XmlWriteHandlerRegistry()
        {
            this.handlers = new Dictionary<Type, XmlWriteHandlerRegistration>();
        }

        public void Register(in XmlWriteHandlerRegistration reg)
        {
            handlers.Add(reg.TargetType, reg);
        }

        public void Register<TComponent>(string typeId)
        {
            Register(XmlWriteHandlerRegistration.Create<TComponent>(new DefaultWriteHandler<TComponent>().Write, typeId));
            
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