using System.Runtime.Serialization;
using EnTTSharp.Annotations;
using EnttSharp.Entities;
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
                             .With(new XmlDataContractRegistrationHandler<EntityKey>())
                             .RegisterEntitiesFromAllAssemblies();
            components.Count.Should().Be(1);

            var xmlReadRegistry = new XmlReadHandlerRegistry();
            xmlReadRegistry.RegisterRange(components);

            var xmlWriteRegistry = new XmlWriteHandlerRegistry();
            xmlWriteRegistry.RegisterRange(components);
        }

        void Test()
        {
           // serializer = new DataContractSerializer(typeof(TData), ds);

        }
    }
}