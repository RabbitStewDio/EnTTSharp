using System;
using System.IO;
using System.Linq;
using EnTTSharp.Annotations;
using EnTTSharp.Annotations.Impl;
using EnTTSharp.Entities;
using EnTTSharp.Serialization;
using EnTTSharp.Serialization.Binary;
using EnTTSharp.Serialization.Binary.AutoRegistration;
using EnTTSharp.Serialization.Binary.Impl;
using FluentAssertions;
using MessagePack;
using MessagePack.Resolvers;
using NUnit.Framework;

namespace EnTTSharp.Test.Serialisation.NestedKeys
{
    public class NestedKeyBinaryTest
    {
        EntityRegistry<EntityKey> CreteEntityRegistry()
        {
            var scanner = new EntityRegistrationScanner(new ComponentRegistrationHandler<EntityKey>());
            if (!scanner.TryRegisterEntity<NestedKeyComponent>(out var reg))
            {
                Assert.Fail();
            }

            var registry = new EntityRegistry<EntityKey>(EntityKey.MaxAge, EntityKey.Create);
            registry.Register(reg);
            return registry;
        }

        [Test]
        public void TestBinarySerialization()
        {
            var registry = CreteEntityRegistry();
            var parent = registry.Create();
            var child = registry.CreateAsActor().AssignComponent(new NestedKeyComponent(parent));

            var xmlString = Serialize(registry);
            xmlString.Length.Should().Be(77);

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

        byte[] Serialize(EntityRegistry<EntityKey> registry)
        {
            var scanner = new EntityRegistrationScanner().With(new BinaryEntityRegistrationHandler());
            if (!scanner.TryRegisterEntity<NestedKeyComponent>(out var registration))
            {
                Assert.Fail();
            }

            var handlerRegistry = new BinaryWriteHandlerRegistry();
            handlerRegistry.Register(registration);

            var resolver = CompositeResolver.Create(
                new EntityKeyResolver(),
                StandardResolver.Instance
            );

            var msgPackOptions = MessagePackSerializerOptions.Standard
                                                             .WithResolver(resolver)
                                                             .WithCompression(MessagePackCompression.None);
            var stream = new MemoryStream();
            var writer = new BinaryArchiveWriter<EntityKey>(handlerRegistry, stream, msgPackOptions);
            using (var snapshot = registry.CreateSnapshot())
            {
                snapshot.WriteAll(writer);
            }

            stream.Flush();

            return stream.ToArray();
        }

        void Deserialize(EntityRegistry<EntityKey> registry, byte[] data)
        {
            var scanner = new EntityRegistrationScanner().With(new BinaryEntityRegistrationHandler());
            if (!scanner.TryRegisterEntity<NestedKeyComponent>(out var registration))
            {
                Assert.Fail();
            }

            var handlerRegistry = new BinaryReadHandlerRegistry();
            handlerRegistry.Register(registration);

            var readerBackend = new BinaryReaderBackend<EntityKey>(handlerRegistry);

            using (var loader = registry.CreateLoader())
            {
                var resolver = CompositeResolver.Create(
                    new EntityKeyResolver(loader.Map),
                    StandardResolver.Instance
                );

                var msgPackOptions = MessagePackSerializerOptions.Standard.WithResolver(resolver);
                var reader = new BinaryBulkArchiveReader<EntityKey>(readerBackend, msgPackOptions);

                var stream = new MemoryStream(data);

                reader.ReadAll(stream, loader);
            }
        }
    }
}