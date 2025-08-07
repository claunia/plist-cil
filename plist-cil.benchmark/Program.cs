using BenchmarkDotNet.Running;

namespace Claunia.PropertyList.Benchmark;

internal class Program
{
    static void Main(string[] args)
    {
        BenchmarkRunner.Run<BinaryPropertyListParserBenchmarks>();
        BenchmarkRunner.Run<BinaryPropertyListWriterBenchmarks>();
    }
}