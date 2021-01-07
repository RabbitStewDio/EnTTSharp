using EnTTSharp.Entities;
using EnTTSharp.Test.Fixtures;
using FluentAssertions;
using NUnit.Framework;

namespace EnTTSharp.Test.Helpers
{
    public class SparseDictTest
    {
        void Change(IEntityViewControl<EntityKey> view, EntityKey e, in TestStructFixture t)
        {
            view.WriteBack(e, new TestStructFixture(10, t.y));
        }

        [Test]
        public void TestViewProcessing()
        {
            var reg = new EntityRegistry<EntityKey>(EntityKey.MaxAge, EntityKey.Create);
            reg.Register<TestStructFixture>();

            var entity = reg.Create();
            reg.AssignComponent(entity, new TestStructFixture());
            reg.View<TestStructFixture>().Apply(Change);
            reg.GetComponent<TestStructFixture>(entity, out var c);
            c.x.Should().Be(10);
        }
    }
}