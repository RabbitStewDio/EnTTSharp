using System.Runtime.Serialization;

namespace EnTTSharp.Test.Serialisation.Surrogates
{
    [DataContract]
    public class DummyEnumObject
    {
        public static readonly DummyEnumObject OptionA = new DummyEnumObject("A");
        public static readonly DummyEnumObject OptionB = new DummyEnumObject("B");

        DummyEnumObject(string id)
        {
            this.Id = id;
        }

        [DataMember] public string Id { get; }
    }
}