using System;
using MessagePack;
using MessagePack.Formatters;
using System.Diagnostics.CodeAnalysis;

namespace EnTTSharp.Serialization.Binary
{
    public delegate IFormatterResolver? FormatterResolverFactory(IEntityKeyMapper entityMapper);
    public delegate IMessagePackFormatter? MessagePackFormatterFactory(IEntityKeyMapper entityMapper);

    public readonly struct BinaryWriteHandlerRegistration
    {
        public readonly string TypeId;
        public readonly Type TargetType;
        public readonly bool Tag;
        readonly object? preProcessor;
        readonly object? formatterResolverFactory;
        readonly object? messagePackFormatterFactory;

        BinaryWriteHandlerRegistration(string typeId,
                                       Type targetType,
                                       bool tag,
                                       object? preProcessor,
                                       object? resolver,
                                       object? messageFormatter)
        {
            TypeId = typeId;
            TargetType = targetType;
            Tag = tag;
            this.preProcessor = preProcessor;
            formatterResolverFactory = resolver;
            messagePackFormatterFactory = messageFormatter;
        }

        public bool TryGetPreProcessor<TComponent>([MaybeNullWhen(false)] out BinaryPreProcessor<TComponent> fn)
        {
            if (preProcessor is BinaryPreProcessor<TComponent> fnx)
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

        public BinaryWriteHandlerRegistration WithFormatterResolver(FormatterResolverFactory? r)
        {
            return new BinaryWriteHandlerRegistration(TypeId, TargetType, Tag, preProcessor, r, messagePackFormatterFactory);
        }

        public BinaryWriteHandlerRegistration WithMessagePackFormatter(MessagePackFormatterFactory? r)
        {
            return new BinaryWriteHandlerRegistration(TypeId, TargetType, Tag, preProcessor, formatterResolverFactory, r);
        }

        public static BinaryWriteHandlerRegistration Create<TComponent>(bool isTag,
                                                                        BinaryPreProcessor<TComponent>? preProcessor = null)
        {
            return new BinaryWriteHandlerRegistration(typeof(TComponent).FullName, typeof(TComponent), isTag, preProcessor, null, null);
        }

        public static BinaryWriteHandlerRegistration Create<TComponent>(string typeId,
                                                                        bool isTag,
                                                                        BinaryPreProcessor<TComponent>? preProcessor = null)
        {
            if (string.IsNullOrEmpty(typeId))
            {
                typeId = typeof(TComponent).FullName;
            }

            return new BinaryWriteHandlerRegistration(typeId, typeof(TComponent), isTag, preProcessor, null, null);
        }
    }
}