using EnTTSharp.Annotations;
using EnTTSharp.Serialization.BinaryPack;
using EnTTSharp.Serialization.BinaryPack.AutoRegistration;
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
                             .With(new BinaryEntityRegistrationHandler())
                             .RegisterEntitiesFromAllAssemblies();
            components.Count.Should().Be(1);

            var xmlReadRegistry = new BinaryReadHandlerRegistry();
            xmlReadRegistry.RegisterRange(components);

            var xmlWriteRegistry = new BinaryWriteHandlerRegistry();
            xmlWriteRegistry.RegisterRange(components);
        }
    }
}