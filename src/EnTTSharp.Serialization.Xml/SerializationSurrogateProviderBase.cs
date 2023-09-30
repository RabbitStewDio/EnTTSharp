using System;
using System.Runtime.Serialization;

namespace EnTTSharp.Serialization.Xml
{
    public abstract class SerializationSurrogateProviderBase<TObject, TSurrogateType> : ISerializationSurrogateProvider
    {
        public object? GetDeserializedObject(object obj, Type memberType)
        {
            if (obj is TSurrogateType surrogate)
            {
                return GetDeserializedObject(surrogate);
            }

            return obj;
        }

        public abstract TObject? GetDeserializedObject(TSurrogateType surrogate);

        public object? GetObjectToSerialize(object obj, Type surrogateType)
        {
            if (obj is TObject source)
            {
                return GetObjectToSerialize(source);
            }

            return obj;
        }

        public abstract TSurrogateType? GetObjectToSerialize(TObject obj);

        public Type GetSurrogateType(Type memberType)
        {
            if (memberType == typeof(TObject))
            {
                return typeof(TSurrogateType);
            }

            return memberType;
        }
    }
}