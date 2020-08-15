﻿using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace EnTTSharp.Serialization.Xml
{
    public class ObjectSurrogateResolver : ISerializationSurrogateProvider
    {
        readonly Dictionary<Type, ISerializationSurrogateProvider> surrogateMappings;

        public ObjectSurrogateResolver()
        {
            surrogateMappings = new Dictionary<Type, ISerializationSurrogateProvider>();
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
            Console.WriteLine("GetDeserializedObject " + obj + " -> " + targetType);
            if (!surrogateMappings.TryGetValue(targetType, out var reg))
            {
                return obj;
            }

            return reg.GetDeserializedObject(obj, targetType);
        }

        public object GetObjectToSerialize(object obj, Type surrogateType)
        {
            Console.WriteLine("GetObjectToSerialize " + obj + " -> " + surrogateType);
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
                Console.WriteLine("  GetSurrogateType " + targetType);
                return targetType;
            }

            Console.WriteLine("*  GetSurrogateType " + targetType + " -> " + reg.GetSurrogateType(targetType));
            return reg.GetSurrogateType(targetType);
        }
    }
}