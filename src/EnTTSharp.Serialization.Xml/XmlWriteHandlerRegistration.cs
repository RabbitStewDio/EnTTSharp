using System;
using System.Xml;

namespace EnTTSharp.Serialization.Xml
{
    public delegate void WriteHandlerDelegate<TComponent>(XmlWriter writer, TComponent component);

    public readonly struct XmlWriteHandlerRegistration
    {
        readonly object handler;

        XmlWriteHandlerRegistration(Type targetType, string typeId, object handler, bool tag)
        {
            if (string.IsNullOrWhiteSpace(typeId))
            {
                throw new ArgumentException("Type id should never be an empty string.");
            }
            
            this.TargetType = targetType ?? throw new ArgumentNullException(nameof(targetType));
            this.TypeId = typeId;
            this.handler = handler;
            this.Tag = tag;
        }

        public Type TargetType { get; }
        public string TypeId { get; }
        public bool Tag { get; }

        public WriteHandlerDelegate<TData> GetHandler<TData>()
        {
            return (WriteHandlerDelegate<TData>)handler;
        }

        public static XmlWriteHandlerRegistration Create<TData>(WriteHandlerDelegate<TData> handler, bool tag)
        {
            if (handler == null)
            {
                throw new ArgumentNullException(nameof(handler));
            }

            return new XmlWriteHandlerRegistration(typeof(TData), typeof(TData).FullName, handler, tag);
        }

        public static XmlWriteHandlerRegistration Create<TData>(string typeId, WriteHandlerDelegate<TData> handler, bool tag)
        {
            if (handler == null)
            {
                throw new ArgumentNullException(nameof(handler));
            }

            return new XmlWriteHandlerRegistration(typeof(TData), typeId ?? typeof(TData).FullName, handler, tag);
        }
    }
}