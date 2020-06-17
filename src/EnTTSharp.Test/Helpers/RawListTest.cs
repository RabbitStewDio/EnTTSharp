using EnttSharp.Entities.Helpers;
using FluentAssertions;
using NUnit.Framework;

namespace EnTTSharp.Test.Helpers
{
    public class RawListTest
    {
        [Test]
        public void Validate_RawList_Enumerator_When_Empty()
        {
            RawList<int> list = new RawList<int>();
            list.GetEnumerator().MoveNext().Should().BeFalse();
        }
    }
}