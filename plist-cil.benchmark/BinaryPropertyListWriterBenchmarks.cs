using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Attributes.Jobs;

namespace Claunia.PropertyList.Benchmark
{
    [CoreJob]
    [MemoryDiagnoser]
    public class BinaryPropertyListWriterBenchmarks
    {
        NSObject data;

        [GlobalSetup]
        public void Setup()
        {
            data = PropertyListParser.Parse("plist.bin");
        }

        [Benchmark]
        public byte[] WriteLargePropertylistTest()
        {
            return BinaryPropertyListWriter.WriteToArray(data);
        }
    }
}