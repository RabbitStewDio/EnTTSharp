using EnTTSharp.Entities.Systems;
using FluentAssertions;
using NUnit.Framework;
using System;

namespace EnTTSharp.Test
{
    public class EntitySystemReferenceTest
    {
        [Test]
        public void TestStandard()
        {
            Action action = TestStandard;
            EntitySystemReference.CreateSystemDescription(action).Should().Be("EntitySystemReferenceTest#TestStandard");
        }
        
        [Test]
        public void TestClosure()
        {
            // ReSharper disable once NotAccessedVariable
            int _ = 10;
            void AClosure()
            {
                _ += 1;
            }
            
            Action action = AClosure;
            EntitySystemReference.CreateSystemDescription(action).Should().Be("EntitySystemReferenceTest#TestClosure.AClosure");
        }
        
        [Test]
        public void WeirdTestClosure()
        {
            // this is a bit convoluted but hey, better safe than sorry.
            
            // ReSharper disable once NotAccessedVariable
            int _ = 10;
            Action AClosure()
            {
                void BClosure()
                {
                    _ += 1;
                }

                return BClosure;
            }
            
            Action action = AClosure();
            EntitySystemReference.CreateSystemDescription(action).Should().Be("EntitySystemReferenceTest#WeirdTestClosure.BClosure");
        }
    }
}