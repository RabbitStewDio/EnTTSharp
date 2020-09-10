using System;
using System.Collections.Generic;
using EnTTSharp.Annotations;

namespace EnTTSharp.Serialization.Xml
{
    public class XmlReadHandlerRegistry
    {
        readonly Dictionary<string, XmlReadHandlerRegistration> handlers;
        readonly Dictionary<Type, XmlReadHandlerRegistration> handlersByType;

        public XmlReadHandlerRegistry()
        {
            handlers = new Dictionary<string, XmlReadHandlerRegistration>();
            handlersByType = new Dictionary<Type, XmlReadHandlerRegistration>();
        }

        public void RegisterRange(IEnumerable<EntityComponentRegistration> range)
        {
            foreach (var r in range)
            {
                Register(r);
            }
        }

        public IEnumerable<XmlReadHandlerRegistration> Handlers => handlersByType.Values;

        public XmlReadHandlerRegistry Register(EntityComponentRegistration reg)
        {
            if (reg.TryGet(out XmlReadHandlerRegistration r))
            {
                Register(r);
            }

            return this;
        }

        public XmlReadHandlerRegistry RegisterRange(IEnumerable<XmlReadHandlerRegistration> range)
        {
            foreach (var r in range)
            {
                Register(r);
            }

            return this;
        }

        public XmlReadHandlerRegistry Register(XmlReadHandlerRegistration r)
        {
            handlers.Add(r.TypeId, r);
            handlersByType.Add(r.TargetType, r);

            return this;
        }

        public bool TryGetValue(Type typeId, out XmlReadHandlerRegistration o)
        {
            return handlersByType.TryGetValue(typeId, out o);
        }

        public bool TryGetValue(string typeId, out XmlReadHandlerRegistration o)
        {
            return handlers.TryGetValue(typeId, out o);
        }
    }
}