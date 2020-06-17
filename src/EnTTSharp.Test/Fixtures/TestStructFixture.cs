using System;
using System.Runtime.Serialization;
using EnTTSharp.Annotations;

namespace EnTTSharp.Test.Fixtures
{
    [Serializable]
    [DataContract]
    [EntityComponent]
    public readonly struct TestStructFixture
    {
        [DataMember]
        public readonly int x;
        [DataMember]
        public readonly int y;

        public TestStructFixture(int x, int y)
        {
            this.x = x;
            this.y = y;
        }
    }
}