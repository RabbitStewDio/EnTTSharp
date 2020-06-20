using System;
using System.Xml;
using EnttSharp.Entities;

namespace EnTTSharp.Serialization.Xml
{
    public delegate TComponent ReadHandlerDelegate<TComponent>(XmlReader reader);

    public readonly struct XmlReadHandlerRegistration
    {
        public readonly string TypeId;
        public readonly Type TargetType;
        public readonly bool Tag;
        readonly object parserFunction;

        XmlReadHandlerRegistration(string typeId, Type targetType, object parserFunction, bool tag)
        {
            TypeId = typeId;
            TargetType = targetType;
            this.parserFunction = parserFunction;
            this.Tag = tag;
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

        public static XmlReadHandlerRegistration Create<TComponent>(ReadHandlerDelegate<TComponent> handler, bool tag)
        {
            return new XmlReadHandlerRegistration(typeof(TComponent).FullName, typeof(TComponent), handler, tag);
        }

        public static XmlReadHandlerRegistration Create<TComponent>(string id, ReadHandlerDelegate<TComponent> handler, bool tag)
        {
            if (string.IsNullOrEmpty(id))
            {
                id = typeof(TComponent).FullName;
            }

            return new XmlReadHandlerRegistration(id, typeof(TComponent), handler, tag);
        }
    }
}