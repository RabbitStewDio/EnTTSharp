using System;
using System.IO;
using System.Text;
using System.Xml;
using EnTTSharp.Annotations;
using EnTTSharp.Annotations.Impl;
using EnTTSharp.Entities;
using EnTTSharp.Serialization;
using EnTTSharp.Serialization.Xml;
using EnTTSharp.Serialization.Xml.AutoRegistration;
using EnTTSharp.Serialization.Xml.Impl;
using EnTTSharp.Test.Serialisation.Surrogates;
using FluentAssertions;
using NUnit.Framework;

namespace EnTTSharp.Test.Serialisation.NestedKeys
{
    public class NestedKeyXmlTest
    {
        EntityRegistry<EntityKey> CreteEntityRegistry()
        {
            var scanner = new EntityRegistrationScanner(new ComponentRegistrationHandler<EntityKey>());
            if (!scanner.TryRegisterComponent<NestedKeyComponent>(out var reg))
            {
                Assert.Fail();
            }

            var registry = new EntityRegistry<EntityKey>(EntityKey.MaxAge, EntityKey.Create);
            registry.Register(reg);
            return registry;
        }

        [Test]
        public void TestXmlSerialization()
        {
            var registry = CreteEntityRegistry();
            var parent = registry.Create();
            var child = registry.CreateAsActor().AssignComponent(new NestedKeyComponent(parent));

            var xmlString = Serialize(registry);

            Console.WriteLine("----------------------");
            Console.WriteLine(xmlString);
            Console.WriteLine("----------------------");

            registry.Clear();

            Deserialize(registry, xmlString);

            registry.Count.Should().Be(2);
            registry.GetComponent(parent, out NestedKeyComponent _).Should().BeFalse();
            registry.GetComponent(child, out NestedKeyComponent childComponent).Should().BeTrue();
            childComponent.ParentReference.Should().Be(parent);


            var freshRegistry = CreteEntityRegistry();
            freshRegistry.Create();
            Deserialize(freshRegistry, xmlString);

            freshRegistry.Count.Should().Be(3);
            freshRegistry.GetComponent(new EntityKey(1, 1), out NestedKeyComponent _).Should().BeFalse();
            freshRegistry.GetComponent(new EntityKey(1, 2), out NestedKeyComponent freshComponent).Should().BeTrue();
            freshComponent.ParentReference.Should().Be(new EntityKey(1, 1));
        }

        void Deserialize(EntityRegistry<EntityKey> registry, string xmlString)
        {
            using (var loader = registry.CreateLoader())
            {
                var surrogateResolver = new ObjectSurrogateResolver();
                surrogateResolver.Register(new EntityKeySurrogateProvider(loader.Map));
                surrogateResolver.Register(new DummyEnumObjectSurrogateProvider());

                var xmlScanner = new EntityRegistrationScanner(new XmlDataContractRegistrationHandler<EntityKey>(surrogateResolver),
                                                               new XmlEntityRegistrationHandler<EntityKey>(surrogateResolver));
                if (!xmlScanner.TryRegisterComponent<NestedKeyComponent>(out var xmlRegistration))
                {
                    Assert.Fail();
                }

                var readerRegistry = new XmlReadHandlerRegistry();
                readerRegistry.Register(xmlRegistration);

                var reader = new XmlBulkArchiveReader<EntityKey>(readerRegistry);
                var xmlReader = XmlReader.Create(new StringReader(xmlString));
                reader.ReadAll(xmlReader, loader);
            }
        }

        string Serialize(EntityRegistry<EntityKey> registry)
        {
            var surrogateResolver = new ObjectSurrogateResolver();
            surrogateResolver.Register(new EntityKeySurrogateProvider());
            surrogateResolver.Register(new DummyEnumObjectSurrogateProvider());

            var xmlScanner = new EntityRegistrationScanner(new XmlDataContractRegistrationHandler<EntityKey>(surrogateResolver),
                                                           new XmlEntityRegistrationHandler<EntityKey>(surrogateResolver));
            if (!xmlScanner.TryRegisterComponent<NestedKeyComponent>(out var xmlRegistration))
            {
                Assert.Fail();
            }

            var sb = new StringBuilder();
            var xmlWriter = XmlWriter.Create(sb,
                                             new XmlWriterSettings()
                                             {
                                                 Indent = true,
                                                 IndentChars = "  ",
                                                 NamespaceHandling = NamespaceHandling.OmitDuplicates
                                             });
            var writerRegistry = new XmlWriteHandlerRegistry();
            writerRegistry.Register(xmlRegistration);

            var writer = new XmlArchiveWriter<EntityKey>(writerRegistry, xmlWriter);

            registry.CreateSnapshot().WriteAll(writer);
            xmlWriter.Flush();

            return sb.ToString();
        }

        [Test]
        public void TestDataContractResolving()
        {
            var model = new NestedKeyComponent(new EntityKey(100, 200));
            var surrogate = new ObjectSurrogateResolver();
            surrogate.Register(new DummyEnumObjectSurrogateProvider());
            surrogate.Register(new EntityKeySurrogateProvider());

            var sb = new StringBuilder();
            var xmlWriter = XmlWriter.Create(sb,
                                             new XmlWriterSettings()
                                             {
                                                 Indent = true,
                                                 IndentChars = "  ",
                                                 NamespaceHandling = NamespaceHandling.OmitDuplicates
                                             });


            var dc = new DefaultDataContractWriteHandler<NestedKeyComponent>(surrogate);
            dc.Write(xmlWriter, model);
            xmlWriter.Flush();

            Console.WriteLine("----------------------");
            Console.WriteLine(sb.ToString());
            Console.WriteLine("----------------------");

            var xmlReader = XmlReader.Create(new StringReader(sb.ToString()));
            var dr = new DefaultDataContractReadHandler<NestedKeyComponent>(surrogate);
            var loaded = dr.Read(xmlReader);
            loaded.ParentReference.Should().Be(new EntityKey(100, 200));
        }

        [Test]
        public void TestDataContractResolving_With_Mapping()
        {
            EntityKey Map(EntityKeyData s) => new EntityKey((byte)(s.Age + 1), s.Key + 1);

            var model = new NestedKeyComponent(new EntityKey(100, 200));
            var surrogate = new ObjectSurrogateResolver();
            surrogate.Register(new DummyEnumObjectSurrogateProvider());
            surrogate.Register(new EntityKeySurrogateProvider(Map));

            var sb = new StringBuilder();
            var xmlWriter = XmlWriter.Create(sb,
                                             new XmlWriterSettings()
                                             {
                                                 Indent = true,
                                                 IndentChars = "  ",
                                                 NamespaceHandling = NamespaceHandling.OmitDuplicates
                                             });


            var dc = new DefaultDataContractWriteHandler<NestedKeyComponent>(surrogate);
            dc.Write(xmlWriter, model);
            xmlWriter.Flush();

            Console.WriteLine("----------------------");
            Console.WriteLine(sb.ToString());
            Console.WriteLine("----------------------");

            var xmlReader = XmlReader.Create(new StringReader(sb.ToString()));
            var dr = new DefaultDataContractReadHandler<NestedKeyComponent>(surrogate);
            var loaded = dr.Read(xmlReader);
            loaded.ParentReference.Should().Be(new EntityKey(101, 201));
        }
    }
}