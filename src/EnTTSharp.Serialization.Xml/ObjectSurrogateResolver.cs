using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using Serilog;

namespace EnTTSharp.Serialization.Xml
{
    public class ObjectSurrogateResolver : ISerializationSurrogateProvider
    {
        static readonly ILogger logger = LogHelper.ForContext<ObjectSurrogateResolver>();
        readonly Dictionary<Type, ISerializationSurrogateProvider> surrogateMappings;

        public ObjectSurrogateResolver()
        {
            surrogateMappings = new Dictionary<Type, ISerializationSurrogateProvider>();
        }

        public void Register<TTarget, TSurrogate>(SerializationSurrogateProviderBase<TTarget, TSurrogate> provider)
        {
            Register(typeof(TTarget), provider);
        }

        public void Register(Type targetType, ISerializationSurrogateProvider provider)
        {
            if (provider == null)
            {
                throw new ArgumentNullException(nameof(provider));
            }

            if (provider == this)
            {
                throw new ArgumentException("Cannot add self", nameof(provider));
            }

            surrogateMappings[targetType] = provider;
        }

        public object GetDeserializedObject(object obj, Type targetType)
        {
            if (!surrogateMappings.TryGetValue(targetType, out var reg))
            {
                return obj;
            }

            return reg.GetDeserializedObject(obj, targetType);
        }

        public object? GetObjectToSerialize(object? obj, Type surrogateType)
        {
            logger.Verbose("GetObjectToSerialize {Object} of type {SurrogateType}", obj, surrogateType);
            if (obj == null)
            {
                return null;
            }
            
            if (!surrogateMappings.TryGetValue(obj.GetType(), out var reg))
            {
                return obj;
            }

            var result = reg.GetObjectToSerialize(obj, surrogateType);
            return result;
        }

        public Type GetSurrogateType(Type targetType)
        {
            // return targetType;
            if (!surrogateMappings.TryGetValue(targetType, out var reg))
            {
                logger.Verbose("GetSurrogateType retaining original type {TargetType}", targetType);
                return targetType;
            }

            var surrogateType = reg.GetSurrogateType(targetType);
            logger.Verbose("GetSurrogateType mapped type {TargetType} to type {SurrogateType}", targetType, surrogateType);
            return surrogateType;
        }
    }
}