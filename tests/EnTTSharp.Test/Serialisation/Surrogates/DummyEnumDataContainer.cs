using System.Runtime.Serialization;

namespace EnTTSharp.Test.Serialisation.Surrogates
{
    [DataContract]
    public readonly struct DummyEnumDataContainer
    {
        [DataMember] public readonly DummyEnumObject da;

        public DummyEnumDataContainer(DummyEnumObject da)
        {
            this.da = da;
        }
    }
}