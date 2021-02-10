using EnTTSharp.Annotations;
using EnTTSharp.Entities;
using EnTTSharp.Serialization.Xml;
using EnTTSharp.Serialization.Xml.AutoRegistration;
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
            components.Count.Should().Be(2);

            var xmlReadRegistry = new XmlReadHandlerRegistry();
            xmlReadRegistry.RegisterRange(components);

            var xmlWriteRegistry = new XmlWriteHandlerRegistry();
            xmlWriteRegistry.RegisterRange(components);
            
            // todo: Not really a test without asserts
        }
    }
}