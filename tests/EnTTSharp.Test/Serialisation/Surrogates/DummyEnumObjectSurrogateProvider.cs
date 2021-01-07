using System;
using EnTTSharp.Serialization.Xml;

namespace EnTTSharp.Test.Serialisation.Surrogates
{
    public class DummyEnumObjectSurrogateProvider : SerializationSurrogateProviderBase<DummyEnumObject, SurrogateContainer<string>>
    {
        public override DummyEnumObject GetDeserializedObject(SurrogateContainer<string> surrogate)
        {
            if (surrogate.Content == "A") return DummyEnumObject.OptionA;
            if (surrogate.Content == "B") return DummyEnumObject.OptionB;
            throw new ArgumentException();
        }

        public override SurrogateContainer<string> GetObjectToSerialize(DummyEnumObject obj)
        {
            return new SurrogateContainer<string>(obj.Id);
        }
    }
}