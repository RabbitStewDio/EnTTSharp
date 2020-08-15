using System;
using EnTTSharp.Annotations;
using EnTTSharp.Annotations.Impl;
using EnttSharp.Entities;
using EnTTSharp.Entities;
using FluentAssertions;
using NUnit.Framework;

namespace EnTTSharp.Test.Annotations
{
    public class AutoRegistrationTest
    {
        [SetUp]
        public void Setup()
        {
        }

        [Test]
        public void Test1()
        {
            var components = new EntityRegistrationScanner()
                             .With(new ComponentRegistrationHandler<EntityKey>())
                             .RegisterEntitiesFromAllAssemblies();

            foreach (var c in components)
            {
                Console.WriteLine(c);
            }

            components.Count.Should().Be(6);

            var registry = new Entities.EntityRegistry<EntityKey>(EntityKey.MaxAge, EntityKey.Create);
            new EntityComponentActivator<EntityKey>()
                .With(new ComponentRegistrationActivator<EntityKey>())
                .ActivateAll(registry, components);
        }
    }
}