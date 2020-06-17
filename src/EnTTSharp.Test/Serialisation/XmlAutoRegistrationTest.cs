using EnTTSharp.Annotations;
using EnTTSharp.Serialization.Xml;
using FluentAssertions;
using NUnit.Framework;

namespace EnTTSharp.Test.Serialisation
{
    public class XmlAutoRegistrationTest
    {
        [Test]
        public void TestRegisterHandlers()
        {
            var components = new EntityRegistrationScanner()
                             .With(new XmlEntityRegistrationHandler())
                             .With(new XmlDataContractRegistrationHandler())
                             .RegisterEntitiesFromAllAssemblies();
            components.Count.Should().Be(1);

            var xmlReadRegistry = new XmlReadHandlerRegistry();
            xmlReadRegistry.RegisterRange(components);

            var xmlWriteRegistry = new XmlWriteHandlerRegistry();
            xmlWriteRegistry.RegisterRange(components);
        }
    }
}