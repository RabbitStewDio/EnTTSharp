using System;
using System.Xml;

namespace EnTTSharp.Serialization.Xml
{
    public delegate void WriteHandlerDelegate<TComponent>(XmlWriter writer, TComponent component);

    public readonly struct XmlWriteHandlerRegistration
    {
        readonly object handler;

        XmlWriteHandlerRegistration(Type targetType, string typeId, object handler, bool tag, object surrogateResolver)
        {
            if (string.IsNullOrWhiteSpace(typeId))
            {
                throw new ArgumentException("Type id should never be an empty string.");
            }
            
            this.TargetType = targetType ?? throw new ArgumentNullException(nameof(targetType));
            this.TypeId = typeId;
            this.handler = handler;
            this.Tag = tag;
            this.surrogateResolver = surrogateResolver;
        }

        public Type TargetType { get; }
        public string TypeId { get; }
        public bool Tag { get; }
        readonly object surrogateResolver;

        public WriteHandlerDelegate<TData> GetHandler<TData>()
        {
            return (WriteHandlerDelegate<TData>)handler;
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

        public XmlWriteHandlerRegistration WithFormatterResolver(FormatterResolverFactory r)
        {
            return new XmlWriteHandlerRegistration(TargetType, TypeId, handler, Tag, r);
        }

        public static XmlWriteHandlerRegistration Create<TData>(WriteHandlerDelegate<TData> handler, bool tag)
        {
            if (handler == null)
            {
                throw new ArgumentNullException(nameof(handler));
            }

            return new XmlWriteHandlerRegistration(typeof(TData), typeof(TData).FullName, handler, tag, null);
        }

        public static XmlWriteHandlerRegistration Create<TData>(string typeId, WriteHandlerDelegate<TData> handler, bool tag)
        {
            if (handler == null)
            {
                throw new ArgumentNullException(nameof(handler));
            }

            return new XmlWriteHandlerRegistration(typeof(TData), typeId ?? typeof(TData).FullName, handler, tag, null);
        }
    }
}