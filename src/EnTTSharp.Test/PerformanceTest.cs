using System;
using System.Diagnostics;
using EnttSharp.Entities;
using NUnit.Framework;

namespace EnTTSharp.Test
{
    struct PerformanceTraitPos
    {
        public int X;
        public int Y;
    }

    struct PerformanceTraitVelocity
    {
        public int X;
        public int Y;

        public PerformanceTraitVelocity(int x, int y)
        {
            this.X = x;
            this.Y = y;
        }

        public static PerformanceTraitVelocity CreateRandom()
        {
            var random = new Random();
            return new PerformanceTraitVelocity(random.Next(50), random.Next(50));
        }
    }

    public class PerformanceTest
    {
        [Test]
        public void TestPerformance()
        {
            var r = new EntityRegistry();
            r.Register<PerformanceTraitPos>();
            r.Register<PerformanceTraitVelocity>();
            var view = r.PersistentView<PerformanceTraitPos, PerformanceTraitVelocity>();
            for (int i = 0; i < 100_000; i += 1)
            {
                r.CreateAsActor()
                 .AssignComponent<PerformanceTraitVelocity>()
                 .AssignComponent<PerformanceTraitPos>();
                if ((i % 1000) == 0)
                {
                    Console.WriteLine("Setup " + i);
                }
            }

            Console.WriteLine("Setup complete");
            Stopwatch w = Stopwatch.StartNew();
            for (int v = 0; v < 1; v += 1)
            {
                view.Apply(Run);
            }

            var time = w.Elapsed;
            Console.WriteLine(time);
        }

        void Run(IEntityViewControl v, EntityKey k, in PerformanceTraitPos c1, in PerformanceTraitVelocity c2)
        {
            var c = c1;
            c.X += c2.X;
            c.Y += c2.Y;
            v.WriteBack(k, in c1);
        }
    }
}