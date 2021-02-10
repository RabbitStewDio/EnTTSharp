using System;
using System.Runtime.Serialization;
using System.Xml;

namespace EnTTSharp.Serialization.Xml
{
    public delegate ISerializationSurrogateProvider FormatterResolverFactory(IEntityKeyMapper entityMapper);
    public delegate TComponent ReadHandlerDelegate<TComponent>(XmlReader reader);

    public readonly struct XmlReadHandlerRegistration
    {
        public readonly string TypeId;
        public readonly Type TargetType;
        public readonly bool Tag;
        readonly object parserFunction;
        readonly object surrogateResolver;

        XmlReadHandlerRegistration(string typeId, Type targetType, object parserFunction, bool tag, object surrogateResolver)
        {
            TypeId = typeId;
            TargetType = targetType;
            this.parserFunction = parserFunction;
            this.Tag = tag;
            this.surrogateResolver = surrogateResolver;
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

        public bool TryGetResolverFactory(out FormatterResolverFactory fn)
        {
            if (surrogateResolver is FormatterResolverFactory fnx)
            {
                fn = fnx;
                return true;
            }

            fn = default;
            return false;
        }

        public XmlReadHandlerRegistration WithFormatterResolver(FormatterResolverFactory r)
        {
            return new XmlReadHandlerRegistration(TypeId, TargetType, parserFunction, Tag, r);
        }

        public static XmlReadHandlerRegistration Create<TComponent>(ReadHandlerDelegate<TComponent> handler, bool tag)
        {
            return new XmlReadHandlerRegistration(typeof(TComponent).FullName, typeof(TComponent), handler, tag, null);
        }

        public static XmlReadHandlerRegistration Create<TComponent>(string id, ReadHandlerDelegate<TComponent> handler, bool tag)
        {
            if (string.IsNullOrEmpty(id))
            {
                id = typeof(TComponent).FullName;
            }

            return new XmlReadHandlerRegistration(id, typeof(TComponent), handler, tag, null);
        }
    }
}