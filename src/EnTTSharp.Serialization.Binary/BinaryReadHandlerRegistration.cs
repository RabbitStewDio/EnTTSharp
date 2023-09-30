using System;
using System.Diagnostics.CodeAnalysis;

namespace EnTTSharp.Serialization.Binary
{
    public delegate TComponent BinaryPostProcessor<TComponent>(in TComponent data);
    public delegate TComponent BinaryPreProcessor<TComponent>(in TComponent data);

    public readonly struct BinaryReadHandlerRegistration
    {
        public readonly string TypeId;
        public readonly Type TargetType;
        public readonly bool Tag;
        readonly object? postProcessor;
        readonly object? formatterResolverFactory;
        readonly object? messagePackFormatterFactory;

        BinaryReadHandlerRegistration(string typeId, 
                                      Type targetType, 
                                      bool tag, 
                                      object? postProcessor, 
                                      object? resolverFactory,
                                      object? messageFormatter)
        {
            TypeId = typeId;
            TargetType = targetType;
            Tag = tag;
            this.postProcessor = postProcessor;
            formatterResolverFactory = resolverFactory;
            this.messagePackFormatterFactory = messageFormatter;
        }

        public bool TryGetPostProcessor<TComponent>([MaybeNullWhen(false)] out BinaryPostProcessor<TComponent> fn)
        {
            if (postProcessor is BinaryPostProcessor<TComponent> fnx)
            {
                fn = fnx;
                return true;
            }

            fn = default;
            return false;
        }

        public bool TryGetResolverFactory([MaybeNullWhen(false)] out FormatterResolverFactory fn)
        {
            if (formatterResolverFactory is FormatterResolverFactory fnx)
            {
                fn = fnx;
                return true;
            }

            fn = default;
            return false;
        }

        public bool TryGetMessagePackFormatterFactory([MaybeNullWhen(false)] out MessagePackFormatterFactory fn)
        {
            if (formatterResolverFactory is MessagePackFormatterFactory fnx)
            {
                fn = fnx;
                return true;
            }

            fn = default;
            return false;
        }

        public BinaryReadHandlerRegistration WithFormatterResolver(FormatterResolverFactory? r)
        {
            return new BinaryReadHandlerRegistration(TypeId, TargetType, Tag, postProcessor, r, messagePackFormatterFactory);
        }

        public BinaryReadHandlerRegistration WithMessagePackFormatter(MessagePackFormatterFactory? r)
        {
            return new BinaryReadHandlerRegistration(TypeId, TargetType, Tag, postProcessor, formatterResolverFactory, r);
        }

        public static BinaryReadHandlerRegistration Create<TComponent>(bool isTag, 
                                                                       BinaryPostProcessor<TComponent>? postProcessor = null)
        {
            return new BinaryReadHandlerRegistration(typeof(TComponent).FullName, typeof(TComponent), isTag, postProcessor, null, null);
        }

        public static BinaryReadHandlerRegistration Create<TComponent>(string typeId, 
                                                                       bool isTag, 
                                                                       BinaryPostProcessor<TComponent>? postProcessor = null)
        {
            if (string.IsNullOrEmpty(typeId))
            {
                typeId = typeof(TComponent).FullName;
            }
            return new BinaryReadHandlerRegistration(typeId, typeof(TComponent), isTag, postProcessor, null, null);
        }
    }
}