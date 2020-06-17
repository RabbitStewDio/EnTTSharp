using System;
using EnTTSharp.Annotations;
using EnTTSharp.Annotations.Impl;
using EnttSharp.Entities;
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
                             .With(new ComponentRegistrationHandler())
                             .RegisterEntitiesFromAllAssemblies();

            foreach (var c in components)
            {
                Console.WriteLine(c);
            }

            components.Count.Should().Be(6);

            var registry = new EntityRegistry();
            new EntityComponentActivator()
                .With(new ComponentRegistrationActivator())
                .ActivateAll(registry, components);
        }
    }
}