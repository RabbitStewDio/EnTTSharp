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
            int x = 10;
            void AClosure()
            {
                x += 1;
            }
            
            Action action = AClosure;
            EntitySystemReference.CreateSystemDescription(action).Should().Be("EntitySystemReferenceTest#TestClosure.AClosure");
        }
        
        [Test]
        public void WeirdTestClosure()
        {
            // this is a bit convoluted but hey, better safe than sorry.
            
            int x = 10;
            Action AClosure()
            {
                void BClosure()
                {
                    x += 1;
                }

                return BClosure;
            }
            
            Action action = AClosure();
            EntitySystemReference.CreateSystemDescription(action).Should().Be("EntitySystemReferenceTest#WeirdTestClosure.BClosure");
        }
    }
}