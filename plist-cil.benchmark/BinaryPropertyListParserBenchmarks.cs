using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Jobs;
using System.IO;

namespace Claunia.PropertyList.Benchmark
{
    [SimpleJob(RuntimeMoniker.NetCoreApp50)]
    [MemoryDiagnoser]
    public class BinaryPropertyListParserBenchmarks
    {
        byte[] data;

        [GlobalSetup]
        public void Setup()
        {
            data = File.ReadAllBytes("plist.bin");
        }

        [Benchmark]
        public NSObject ReadLargePropertylistTest()
        {
            NSObject nsObject = PropertyListParser.Parse(data);
            return nsObject;
        }
    }
}