using BenchmarkDotNet.Configs;
using BenchmarkDotNet.Diagnosers;
using BenchmarkDotNet.Running;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;

namespace EnTTSharp.Benchmarks
{
#pragma warning disable 162
    [SuppressMessage("ReSharper", "HeuristicUnreachableCode")]
    public class MainClass
    {
        const bool RunManually = false;

        [SuppressMessage("ReSharper", "ConditionIsAlwaysTrueOrFalse")]
        public static void Main(string[] args)
        {
            if (RunManually)
            { 
                RunOnce_GoalFinder();
                return;
            }

            var config = new ManualConfig();
            config.Add(DefaultConfig.Instance);
            config.Add(MemoryDiagnoser.Default);
            if (IsAdmin())
            {
                config.Add(HardwareCounter.BranchMispredictions, HardwareCounter.BranchInstructions);
            }
            
            BenchmarkRunner.Run(typeof(MainClass).Assembly, config);
        }

        static bool IsAdmin()
        {
            return false;
        }

        static void RunOnce_GoalFinder()
        {
            var bm = new BasicModifyLoopBenchmark();
            bm.SetUp();
            Stopwatch sw = Stopwatch.StartNew();
            for (int i = 0; i < 20000; i += 1)
            {
                if ((i % 50) == 0)
                {
                    //MemoryProfiler.GetSnapshot();
                }

                sw.Restart();
                bm.IterateMultiThreadedThreadedRef();
                //bm.IterateSingleThreadedWriteBack();
                // Console.WriteLine(i + " " + sw.Elapsed);
            }
        }
    }
}
#pragma warning restore 162
