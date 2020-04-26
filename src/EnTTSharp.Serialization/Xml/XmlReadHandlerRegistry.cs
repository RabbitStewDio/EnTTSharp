using System.Collections.Generic;

namespace EnTTSharp.Serialization.Xml
{
    public class XmlReadHandlerRegistry
    {
        readonly Dictionary<string, XmlReadHandlerRegistration> handlers;

        public XmlReadHandlerRegistry()
        {
            handlers = new Dictionary<string, XmlReadHandlerRegistration>();
        }

        public void Register(XmlReadHandlerRegistration r)
        {
            handlers.Add(r.TypeId, r);
        }

        public bool TryGetValue(string typeId, out XmlReadHandlerRegistration o)
        {
            return handlers.TryGetValue(typeId, out o);
        }
    }
}