using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Attributes.Jobs;
using System.IO;

namespace Claunia.PropertyList.Benchmark
{
    [CoreJob]
    [MemoryDiagnoser]
    public class BinaryPropertyListParserBenchmarks
    {
        private byte[] data = null;

        [GlobalSetup]
        public void Setup()
        {
            data = File.ReadAllBytes("plist.bin");
        }

        [Benchmark]
        public NSObject ReadLargePropertylistTest()
        {
            var nsObject = PropertyListParser.Parse(this.data);
            return nsObject;
        }
    }
}
