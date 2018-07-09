using BenchmarkDotNet.Reports;
using BenchmarkDotNet.Running;

namespace Claunia.PropertyList.Benchmark
{
    class Program
    {
        static void Main(string[] args)
        {
            Summary summary = BenchmarkRunner.Run<BinaryPropertyListParserBenchmarks>();
            summary = BenchmarkRunner.Run<BinaryPropertyListWriterBenchmarks>();
        }
    }
}