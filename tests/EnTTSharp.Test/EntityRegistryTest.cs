using EnTTSharp.Entities;
using FluentAssertions;
using NUnit.Framework;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace EnTTSharp.Test
{
    [SuppressMessage("ReSharper", "NotNullOrRequiredMemberIsNotInitialized")]
    public class EntityRegistryTest
    {
        EntityRegistry<EntityKey> registry;
        IReadOnlyList<EntityKey> keys;
        IPersistentEntityView<EntityKey, int> view;

        [SetUp]
        public void SetUp()
        {
            registry = new EntityRegistry<EntityKey>(EntityKey.MaxAge, (age, id) => new EntityKey(age, id));
            registry.RegisterNonConstructable<int>();
            var k1 = registry.CreateAsActor().AssignComponent(1).Entity;
            var k2 = registry.CreateAsActor().AssignComponent(2).Entity;
            var k3 = registry.CreateAsActor().AssignComponent(3).Entity;
            var k4 = registry.CreateAsActor().Entity;
            keys = new[] { k1, k2, k3, k4 };
            view = registry.PersistentView<int>();
        }

        [Test]
        public void TestClear()
        {
            view.Count.Should().Be(3);
            registry.Count.Should().Be(4);

            registry.Clear();

            view.Count.Should().Be(0);
            registry.Count.Should().Be(0);

            var pool = registry.GetPool<int>();
            pool.Contains(keys[0]).Should().BeFalse();
            pool.Contains(keys[1]).Should().BeFalse();
            pool.Contains(keys[2]).Should().BeFalse();
            pool.Contains(keys[3]).Should().BeFalse();
        }

        [Test]
        public void TestRemoveComponentFirst()
        {
            registry.RemoveComponent<int>(keys[0]);

            view.Should().BeEquivalentTo(keys[1], keys[2]);
            registry.Should().BeEquivalentTo(keys);
            view.CollectContents().Should().BeEquivalentTo((keys[1], 2), (keys[2], 3));
        }

        [Test]
        public void TestRemoveEntityFirst()
        {
            registry.Destroy(keys[0]);

            view.Should().BeEquivalentTo(keys[1], keys[2]);
            registry.Should().BeEquivalentTo(keys[1], keys[2], keys[3]);
            view.CollectContents().Should().BeEquivalentTo((keys[1], 2), (keys[2], 3));
            
            var pool = registry.GetPool<int>();
            pool.Contains(keys[0]).Should().BeFalse();
        }

        [Test]
        public void TestRemoveComponentLast()
        {
            registry.RemoveComponent<int>(keys[2]);

            view.Should().BeEquivalentTo(keys[0], keys[1]);
            registry.Should().BeEquivalentTo(keys);
            view.CollectContents().Should().BeEquivalentTo((keys[0], 1), (keys[1], 2));
        }

        [Test]
        public void TestRemoveEntityLast()
        {
            registry.Destroy(keys[2]);

            view.Should().BeEquivalentTo(keys[0], keys[1]);
            registry.Should().BeEquivalentTo(keys[0], keys[1], keys[3]);
            view.CollectContents().Should().BeEquivalentTo((keys[0], 1), (keys[1], 2));

            var pool = registry.GetPool<int>();
            pool.Contains(keys[2]).Should().BeFalse();
        }
    }

    public static class ComponentPoolHelpers
    {
        public static List<(EntityKey, T)> CollectContents<T>(this IEntityView<EntityKey, T> view)
        {
            void CollectData(IEntityViewControl<EntityKey> v, List<(EntityKey, T)> context, EntityKey k, in T data)
            {
                context.Add((k, data));
            }

            var collector = new List<(EntityKey, T)>();
            view.ApplyWithContext(collector, CollectData);
            return collector;
        }

        public static List<(EntityKey, T1, T2)> CollectContents<T1, T2>(this IEntityView<EntityKey, T1, T2> view)
        {
            void CollectData(IEntityViewControl<EntityKey> v, List<(EntityKey, T1, T2)> context, EntityKey k, in T1 data, in T2 data2)
            {
                context.Add((k, data, data2));
            }

            var collector = new List<(EntityKey, T1, T2)>();
            view.ApplyWithContext(collector, CollectData);
            return collector;
        }
    }
}