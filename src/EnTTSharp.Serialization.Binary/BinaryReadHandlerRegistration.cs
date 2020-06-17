using System;

namespace EnTTSharp.Serialization.BinaryPack
{
    public delegate TComponent BinaryPostProcessor<TComponent>(in TComponent data);

    public readonly struct BinaryReadHandlerRegistration
    {
        public readonly string TypeId;
        public readonly Type TargetType;
        public readonly bool Tag;
        public readonly object PostProcessor;

        BinaryReadHandlerRegistration(string typeId, Type targetType, bool tag, object postProcessor)
        {
            TypeId = typeId;
            TargetType = targetType;
            Tag = tag;
            PostProcessor = postProcessor;
        }

        public bool TryGetPostProcessor<TComponent>(out BinaryPostProcessor<TComponent> fn)
        {
            if (PostProcessor is BinaryPostProcessor<TComponent> fnx)
            {
                fn = fnx;
                return true;
            }

            fn = default;
            return false;
        }

        public static BinaryReadHandlerRegistration Create<TComponent>(bool isTag, BinaryPostProcessor<TComponent> postProcessor = null)
        {
            return new BinaryReadHandlerRegistration(typeof(TComponent).FullName, typeof(TComponent), isTag, postProcessor);
        }

        public static BinaryReadHandlerRegistration Create<TComponent>(string typeId, bool isTag, BinaryPostProcessor<TComponent> postProcessor = null)
        {
            if (string.IsNullOrEmpty(typeId))
            {
                typeId = typeof(TComponent).FullName;
            }
            return new BinaryReadHandlerRegistration(typeId, typeof(TComponent), isTag, postProcessor);
        }
    }
}