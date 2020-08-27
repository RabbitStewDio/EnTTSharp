using EnTTSharp.Entities;
using EnTTSharp.Entities.Helpers;
using FluentAssertions;
using NUnit.Framework;

namespace EnTTSharp.Test.Helpers
{
    public class SparseSetTest
    {
        [Test]
        public void Adding()
        {
            var s = new SparseSet<EntityKey>();
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

        [Test]
        public void Capacity()
        {
            var s = new SparseSet<EntityKey>();
            s.Reserve(42);

            s.Capacity.Should().Be(42);
            s.Count.Should().Be(0);
            s.Contains(EntityKey.Create(0, 0)).Should().BeFalse();
            s.Contains(EntityKey.Create(0, 42)).Should().BeFalse();
        }

        [Test]
        public void AddOne()
        {
            var s = new SparseSet<EntityKey>();
            s.Reserve(42);
            s.Add(EntityKey.Create(0, 42));

            s.Count.Should().Be(1);
            s.Contains(EntityKey.Create(0, 0)).Should().BeFalse();
            s.Contains(EntityKey.Create(0, 42)).Should().BeTrue();
            s.IndexOf(EntityKey.Create(0, 42)).Should().Be(0);
        }

        [Test]
        public void RemoveOne()
        {
            var s = new SparseSet<EntityKey>();
            s.Reserve(42);
            s.Add(EntityKey.Create(0, 42));
            s.Remove(EntityKey.Create(0, 42));

            s.Count.Should().Be(0);
            s.Contains(EntityKey.Create(0, 0)).Should().BeFalse();
            s.Contains(EntityKey.Create(0, 42)).Should().BeFalse();
            s.IndexOf(EntityKey.Create(0, 42)).Should().Be(-1);
        }

        [Test]
        public void AddRemoveAddOne()
        {
            var s = new SparseSet<EntityKey>();
            s.Reserve(42);
            s.Add(EntityKey.Create(0, 42));
            s.Remove(EntityKey.Create(0, 42));
            s.Add(EntityKey.Create(0, 42));

            s.Count.Should().Be(1);
            s.Contains(EntityKey.Create(0, 0)).Should().BeFalse();
            s.Contains(EntityKey.Create(0, 42)).Should().BeTrue();
            s.IndexOf(EntityKey.Create(0, 42)).Should().Be(0);
        }
    }
}