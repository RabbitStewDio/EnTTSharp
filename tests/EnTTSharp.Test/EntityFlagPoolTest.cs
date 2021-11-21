using EnTTSharp.Entities;
using NUnit.Framework;

namespace EnTTSharp.Test
{
    public class EntityFlagPoolTest
    {
        EntityRegistry<EntityKey> registry;
        EntityKey[] keys;
        IPersistentEntityView<EntityKey, int> componentPool;
        IPersistentEntityView<EntityKey, SomeMarker> flagPool;
        IPersistentEntityView<EntityKey, int, SomeMarker> combinedPool;

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
            componentPool = registry.PersistentView<int>();
            flagPool = registry.PersistentView<SomeMarker>();
            combinedPool = registry.PersistentView<int, SomeMarker>();
        }

        [Test]
        public void TestClear()
        {
            registry.Clear();
        }

        readonly struct SomeMarker
        {
        }
    }
}