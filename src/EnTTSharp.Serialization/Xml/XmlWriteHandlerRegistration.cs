using System;
using System.Xml;
using EnttSharp.Entities;

namespace EnTTSharp.Serialization.Xml
{
    public delegate void WriteHandlerDelegate<TComponent>(XmlWriter writer, EntityKey k, TComponent component);

    public readonly struct XmlWriteHandlerRegistration
    {
        readonly object handler;

        XmlWriteHandlerRegistration(Type targetType, string typeId, object handler)
        {
            this.TargetType = targetType;
            this.TypeId = typeId;
            this.handler = handler;
        }

        public Type TargetType { get; }
        public string TypeId { get; }

        public WriteHandlerDelegate<TData> GetHandler<TData>()
        {
            return (WriteHandlerDelegate<TData>)handler;
        }

        public static XmlWriteHandlerRegistration Create<TData>(WriteHandlerDelegate<TData> handler)
        {
            if (handler == null)
            {
                throw new ArgumentNullException(nameof(handler));
            }

            return new XmlWriteHandlerRegistration(typeof(TData), typeof(TData).FullName, handler);
        }

        public static XmlWriteHandlerRegistration Create<TData>(WriteHandlerDelegate<TData> handler, string typeId)
        {
            if (handler == null)
            {
                throw new ArgumentNullException(nameof(handler));
            }

            return new XmlWriteHandlerRegistration(typeof(TData), typeId ?? typeof(TData).FullName, handler);
        }
    }
}