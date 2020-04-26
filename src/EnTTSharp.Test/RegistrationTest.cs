using EnTTSharp.Annotations;
using EnTTSharp.Annotations.Impl;
using EnttSharp.Entities;
using FluentAssertions;
using NUnit.Framework;

namespace EnTTSharp.Test
{
    [EntityComponent()]
    public class DefaultConstrucableClassComponent
    {
    }

    [EntityComponent(EntityConstructor.NonConstructable)]
    public class ProhibitDefaultConstrucableClassComponent
    {
    }

    [EntityComponent()]
    public class NonDefaultConstrucableClassComponent
    {
        public NonDefaultConstrucableClassComponent(int dummy)
        {
        }
    }

    [EntityComponent()]
    public struct DefaultConstructableStruct
    {
    }

    [EntityComponent(EntityConstructor.NonConstructable)]
    public struct NonDefaultConstructableStruct
    {
    }

    public class RegistrationTest
    {
        [SetUp]
        public void Setup()
        {
        }

        [Test]
        public void Test1()
        {
            var components = new EntityRegistrationScanner()
                             .With(new ComponentRegistrationHandler())
                             .RegisterEntitiesFromAllAssemblies();
            components.Count.Should().Be(5);

            var registry = new EntityRegistry();
            new EntityComponentActivator()
                .With(new ComponentRegistrationActivator())
                .ActivateAll(registry, components);
        }
    }
}