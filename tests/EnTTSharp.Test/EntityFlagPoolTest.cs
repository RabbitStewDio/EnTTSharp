using EnTTSharp.Entities;
using FluentAssertions;
using NUnit.Framework;

namespace EnTTSharp.Test
{
    public class EntityFlagPoolTest
    {
        EntityRegistry<EntityKey> registry;
        EntityKey[] keys;
        IPersistentEntityView<EntityKey, SomeMarker> view;

        [SetUp]
        public void SetUp()
        {
            registry = new EntityRegistry<EntityKey>(EntityKey.MaxAge, (age, id) => new EntityKey(age, id));
            registry.RegisterNonConstructable<int>();
            registry.RegisterFlag<SomeMarker>();


            var k1 = registry.CreateAsActor().AssignComponent(1).Entity;
            var k2 = registry.CreateAsActor().AssignComponent(2).AssignComponent<SomeMarker>().Entity;
            var k3 = registry.CreateAsActor().AssignComponent(3).AssignComponent<SomeMarker>().Entity;
            var k4 = registry.CreateAsActor().AssignComponent<SomeMarker>().Entity;
            var k5 = registry.CreateAsActor().Entity;

            keys = new[] { k1, k2, k3, k4, k5 };
            view = registry.PersistentView<SomeMarker>();
        }

        [Test]
        public void TestClear()
        {
            view.Count.Should().Be(3);
            registry.Count.Should().Be(5);

            registry.Clear();

            view.Count.Should().Be(0);
            registry.Count.Should().Be(0);

            var pool = registry.GetPool<SomeMarker>();
            pool.Contains(keys[0]).Should().BeFalse();
            pool.Contains(keys[1]).Should().BeFalse();
            pool.Contains(keys[2]).Should().BeFalse();
            pool.Contains(keys[3]).Should().BeFalse();
        }


        [Test]
        public void TestRemoveComponentFirst()
        {
            registry.RemoveComponent<SomeMarker>(keys[1]);

            view.Should().BeEquivalentTo(keys[2], keys[3]);
            registry.Should().BeEquivalentTo(keys);
        }

        [Test]
        public void TestRemoveEntityFirst()
        {
            registry.Destroy(keys[1]);

            view.Should().BeEquivalentTo(keys[2], keys[3]);
            registry.Should().BeEquivalentTo(keys[0], keys[2], keys[3], keys[4]);
            
            var pool = registry.GetPool<SomeMarker>();
            pool.Contains(keys[1]).Should().BeFalse();
        }

        [Test]
        public void TestRemoveComponentLast()
        {
            registry.RemoveComponent<SomeMarker>(keys[3]);

            view.Should().BeEquivalentTo(keys[1], keys[2]);
            registry.Should().BeEquivalentTo(keys);
        }

        [Test]
        public void TestRemoveEntityLast()
        {
            registry.Destroy(keys[3]);

            view.Should().BeEquivalentTo(keys[1], keys[2]);
            registry.Should().BeEquivalentTo(keys[0], keys[1], keys[2], keys[4]);

            var pool = registry.GetPool<SomeMarker>();
            pool.Contains(keys[3]).Should().BeFalse();
        }
        
        readonly struct SomeMarker
        {
        }
    }
}