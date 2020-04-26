using System;
using System.Runtime.Serialization;

namespace EnTTSharp.Test
{
    [Serializable]
    [DataContract]
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