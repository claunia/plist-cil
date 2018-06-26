using Claunia.PropertyList;
using System.IO;
using Xunit;

namespace plistcil.test
{
    public class BinaryPropertyListWriterTests
    {
        [Fact]
        public void RoundtripTest()
        {
            byte[] data = File.ReadAllBytes("test-files/plist.bin");
            NSObject root = PropertyListParser.Parse(data);

            using (MemoryStream actualOutput = new MemoryStream())
            using (Stream expectedOutput = File.OpenRead("test-files/plist.bin"))
            using (ValidatingStream validatingStream = new ValidatingStream(actualOutput, expectedOutput))
            {
                BinaryPropertyListWriter writer = new BinaryPropertyListWriter(validatingStream);
                writer.ReuseObjectIds = false;
                writer.Write(root);
            }
        }

        [Fact]
        public void Roundtrip2Test()
        {
            byte[] data = File.ReadAllBytes("test-files/plist2.bin");
            NSObject root = PropertyListParser.Parse(data);

            using (MemoryStream actualOutput = new MemoryStream())
            using (Stream expectedOutput = File.OpenRead("test-files/plist2.bin"))
            using (ValidatingStream validatingStream = new ValidatingStream(actualOutput, expectedOutput))
            {
                BinaryPropertyListWriter writer = new BinaryPropertyListWriter(validatingStream);
                writer.ReuseObjectIds = false;
                writer.Write(root);
            }
        }

        [Fact]
        public void Roundtrip3Test()
        {
            byte[] data = File.ReadAllBytes("test-files/plist3.bin");
            NSObject root = PropertyListParser.Parse(data);

            using (MemoryStream actualOutput = new MemoryStream())
            using (Stream expectedOutput = File.OpenRead("test-files/plist3.bin"))
            using (ValidatingStream validatingStream = new ValidatingStream(actualOutput, expectedOutput))
            {
                BinaryPropertyListWriter writer = new BinaryPropertyListWriter(validatingStream);
                writer.ReuseObjectIds = false;
                writer.Write(root);
            }
        }
    }
}
