using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Jobs;
using EnTTSharp.Entities;
using System;

namespace EnTTSharp.Benchmarks
{
    [SimpleJob(RuntimeMoniker.Net472)]
    [SimpleJob(RuntimeMoniker.NetCoreApp31, baseline: true)]
    public class BasicModifyLoopBenchmark
    {
        EntityRegistry<EntityKey>? registry;
        IPersistentEntityView<EntityKey, PerformanceTraitVelocity, PerformanceTraitPos>? view;

        [GlobalSetup]
        public void SetUp()
        {
            registry = new EntityRegistry<EntityKey>(EntityKey.MaxAge, EntityKey.Create);
            registry.Register<PerformanceTraitPos>();
            registry.Register<PerformanceTraitVelocity>();
            view = registry.PersistentView<PerformanceTraitVelocity, PerformanceTraitPos>();
            view.AllowParallelExecution = true;

            var rng = new Random();
            for (int i = 0; i < 100_000; i += 1)
            {
                registry.CreateAsActor()
                 .AssignComponent(PerformanceTraitVelocity.CreateRandom(rng))
                 .AssignComponent(PerformanceTraitPos.CreateRandom(rng));
            }
        }

        [Benchmark]
        public void IterateSingleThreadedRef()
        {
            view!.AllowParallelExecution = false;
            view.Apply(RunByRef);
        }

        [Benchmark]
        public void IterateSingleThreadedWriteBack()
        {
            view!.AllowParallelExecution = false;
            view.Apply(RunWriteBack);
        }
        
        [Benchmark(Baseline = true)]
        public void IterateMultiThreadedThreadedRef()
        {
            view!.AllowParallelExecution = true;
            view.Apply(RunByRef);
        }

        [Benchmark]
        public void IterateMultiThreadedWriteBack()
        {
            view!.AllowParallelExecution = true;
            view.Apply(RunWriteBack);
        }
        
        void RunByRef(IEntityViewControl<EntityKey> v, EntityKey k, in PerformanceTraitVelocity c2, ref PerformanceTraitPos c1)
        {
            c1 = new PerformanceTraitPos(c1.X + c2.X, c1.Y + c2.Y);
        }

        void RunWriteBack(IEntityViewControl<EntityKey> v, EntityKey k, in PerformanceTraitVelocity c2, in PerformanceTraitPos c1)
        {
            var cx = new PerformanceTraitPos(c1.X + c2.X, c1.Y + c2.Y);
            v.WriteBack(k, cx);
        }

    }
    
    readonly struct PerformanceTraitPos
    {
        public readonly int X;
        public readonly int Y;

        public PerformanceTraitPos(int x, int y)
        {
            X = x;
            Y = y;
        }

        public static PerformanceTraitPos CreateRandom(Random random)
        {
            return new PerformanceTraitPos(random.Next(50), random.Next(50));
        }
    }

    readonly struct PerformanceTraitVelocity
    {
        public readonly int X;
        public readonly int Y;

        public PerformanceTraitVelocity(int x, int y)
        {
            this.X = x;
            this.Y = y;
        }

        public static PerformanceTraitVelocity CreateRandom(Random random)
        {
            return new PerformanceTraitVelocity(random.Next(50), random.Next(50));
        }
    }

}