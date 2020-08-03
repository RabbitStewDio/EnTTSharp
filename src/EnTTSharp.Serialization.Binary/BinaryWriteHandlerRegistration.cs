using System;

namespace EnTTSharp.Serialization.Binary
{
    public readonly struct BinaryWriteHandlerRegistration
    {
        public readonly string TypeId;
        public readonly Type TargetType;
        public readonly bool Tag;
        public readonly object PreProcessor;

        public BinaryWriteHandlerRegistration(string typeId, Type targetType, bool tag, object preProcessor)
        {
            TypeId = typeId;
            TargetType = targetType;
            Tag = tag;
            PreProcessor = preProcessor;
        }

        public bool TryGetPreProcessor<TComponent>(out BinaryPreProcessor<TComponent> fn)
        {
            if (PreProcessor is BinaryPreProcessor<TComponent> fnx)
            {
                fn = fnx;
                return true;
            }

            fn = default;
            return false;
        }

        public static BinaryWriteHandlerRegistration Create<TComponent>(bool isTag, 
                                                                        BinaryPreProcessor<TComponent> preProcessor = null)
        {
            return new BinaryWriteHandlerRegistration(typeof(TComponent).FullName, typeof(TComponent), isTag, preProcessor);
        }

        public static BinaryWriteHandlerRegistration Create<TComponent>(string typeId, bool isTag, 
                                                                        BinaryPreProcessor<TComponent> preProcessor = null,
                                                                        BinaryPostProcessor<TComponent> postProcessor = null)
        {
            if (string.IsNullOrEmpty(typeId))
            {
                typeId = typeof(TComponent).FullName;
            }

            return new BinaryWriteHandlerRegistration(typeId, typeof(TComponent), isTag, preProcessor);
        }
    }
}