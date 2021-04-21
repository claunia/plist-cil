using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Jobs;

namespace Claunia.PropertyList.Benchmark
{
    [SimpleJob(RuntimeMoniker.NetCoreApp50)]
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