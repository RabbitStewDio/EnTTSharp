using System;
using System.Xml;
using EnttSharp.Entities;

namespace EnTTSharp.Serialization.Xml
{
    public delegate TComponent ReadHandlerDelegate<TComponent>(XmlReader reader, Func<EntityKey,EntityKey> keyMapper);

    public readonly struct XmlReadHandlerRegistration
    {
        public readonly string TypeId;
        public readonly Type TargetType;
        readonly object parserFunction;

        XmlReadHandlerRegistration(string typeId, Type targetType, object parserFunction)
        {
            TypeId = typeId;
            TargetType = targetType;
            this.parserFunction = parserFunction;
        }

        public bool TryGetParser<TComponent>(out ReadHandlerDelegate<TComponent> fn)
        {
            if (parserFunction is ReadHandlerDelegate<TComponent> fnx)
            {
                fn = fnx;
                return true;
            }

            fn = default;
            return false;
        }
        
        public static XmlReadHandlerRegistration Create<TComponent>(ReadHandlerDelegate<TComponent> handler)
        {
            return new XmlReadHandlerRegistration(typeof(TComponent).FullName, typeof(TComponent), handler);
        }

        public static XmlReadHandlerRegistration Create<TComponent>(string id, ReadHandlerDelegate<TComponent> handler)
        {
            if (string.IsNullOrEmpty(id))
            {
                id = typeof(TComponent).FullName;
            }
            return new XmlReadHandlerRegistration(id, typeof(TComponent), handler);
        }
    }
}