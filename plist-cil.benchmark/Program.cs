using BenchmarkDotNet.Running;
using System;

namespace Claunia.PropertyList.Benchmark
{
    class Program
    {
        static void Main(string[] args)
        {
            var summary = BenchmarkRunner.Run<BinaryPropertyListParserBenchmarks>();
        }
    }
}
