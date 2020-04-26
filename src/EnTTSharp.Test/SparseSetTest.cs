using EnttSharp.Entities;
using FluentAssertions;
using NUnit.Framework;

namespace EnTTSharp.Test
{
  public class SparseSetTest
  {
    [Test]
    public void Adding()
    {
      var s = new SparseSet();
      var key1 = new EntityKey(1, 100);
      var key2 = new EntityKey(1, 105);
      var key3 = new EntityKey(1, 0);
      s.Add(key1);
      s.Add(key2);
      s.Add(key3);

      s.Contains(key1).Should().Be(true);
      s.Contains(key2).Should().Be(true);
      s.Contains(key3).Should().Be(true);
      s.Contains(new EntityKey(0, 0)).Should().Be(false);
      s.Contains(new EntityKey(1, 110)).Should().Be(false);

      s.IndexOf(key1).Should().Be(0);
      s.IndexOf(key2).Should().Be(1);
      s.IndexOf(key3).Should().Be(2);
      s.IndexOf(new EntityKey(0, 0)).Should().Be(-1);
      s.IndexOf(new EntityKey(1, 110)).Should().Be(-1);
    }
  }
}