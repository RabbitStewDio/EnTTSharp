using EnTTSharp.Annotations;
using EnTTSharp.Entities;
using EnTTSharp.Serialization.Binary;
using EnTTSharp.Serialization.Binary.AutoRegistration;
using FluentAssertions;
using NUnit.Framework;

namespace EnTTSharp.Test.Serialisation
{
    public class BinaryAutoRegistrationTest
    {
        [Test]
        public void TestRegisterHandlers()
        {
            var components = new EntityRegistrationScanner()
                             .With(new BinaryEntityRegistrationHandler<EntityKey>())
                             .RegisterEntitiesFromAllAssemblies();
            components.Count.Should().Be(2);

            var xmlReadRegistry = new BinaryReadHandlerRegistry();
            xmlReadRegistry.RegisterRange(components);

            var xmlWriteRegistry = new BinaryWriteHandlerRegistry();
            xmlWriteRegistry.RegisterRange(components);
        }
    }
}