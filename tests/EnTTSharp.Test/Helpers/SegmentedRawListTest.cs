using EnTTSharp.Entities.Helpers;
using FluentAssertions;
using NUnit.Framework;

namespace EnTTSharp.Test.Helpers
{
    public class SegmentedRawListTest
    {
        [Test]
        public void Validate_RawList_Enumerator_When_Empty()
        {
            SegmentedRawList<int> list = new SegmentedRawList<int>();
            list.GetEnumerator().MoveNext().Should().BeFalse();
        }

        [Test]
        public void ValidateAdd()
        {
            SegmentedRawList<int> list = new SegmentedRawList<int>(2, 10);
            list.Add(0);
        }
    }
}