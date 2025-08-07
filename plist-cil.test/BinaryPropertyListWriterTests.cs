using System.IO;
using Claunia.PropertyList;
using Xunit;

namespace plistcil.test;

public class BinaryPropertyListWriterTests
{
    [Fact]
    public void Roundtrip2Test()
    {
        byte[]   data = File.ReadAllBytes("test-files/plist2.bin");
        NSObject root = PropertyListParser.Parse(data);

        using var actualOutput = new MemoryStream();

        using Stream expectedOutput = File.OpenRead("test-files/plist2.bin");

        using var validatingStream = new ValidatingStream(actualOutput, expectedOutput);

        var writer = new BinaryPropertyListWriter(validatingStream)
        {
            ReuseObjectIds = false
        };

        writer.Write(root);
    }

    [Fact]
    public void Roundtrip3Test()
    {
        byte[]   data = File.ReadAllBytes("test-files/plist3.bin");
        NSObject root = PropertyListParser.Parse(data);

        using var actualOutput = new MemoryStream();

        using Stream expectedOutput = File.OpenRead("test-files/plist3.bin");

        using var validatingStream = new ValidatingStream(actualOutput, expectedOutput);

        var writer = new BinaryPropertyListWriter(validatingStream)
        {
            ReuseObjectIds = false
        };

        writer.Write(root);
    }

    [Fact]
    public void Roundtrip4Test()
    {
        byte[]   data = File.ReadAllBytes("test-files/plist4.bin");
        NSObject root = PropertyListParser.Parse(data);

        using var actualOutput = new MemoryStream();

        using Stream expectedOutput = File.OpenRead("test-files/plist4.bin");

        using var validatingStream = new ValidatingStream(actualOutput, expectedOutput);

        var writer = new BinaryPropertyListWriter(validatingStream)
        {
            ReuseObjectIds = false
        };

        writer.Write(root);
    }

    [Fact]
    public void RoundtripTest()
    {
        byte[]   data = File.ReadAllBytes("test-files/plist.bin");
        NSObject root = PropertyListParser.Parse(data);

        using var actualOutput = new MemoryStream();

        using Stream expectedOutput = File.OpenRead("test-files/plist.bin");

        using var validatingStream = new ValidatingStream(actualOutput, expectedOutput);

        var writer = new BinaryPropertyListWriter(validatingStream)
        {
            ReuseObjectIds = false
        };

        writer.Write(root);
    }
}