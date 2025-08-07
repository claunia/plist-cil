using Claunia.PropertyList;
using Xunit;

namespace plistcil.test;

public class BinaryPropertyListParserTests
{
    [Theory]
    [InlineData(new byte[]
                {
                    0x08
                },
                0x08)]
    [InlineData(new byte[]
                {
                    0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x07
                },
                7)]
    [InlineData(new byte[]
                {
                    0x00, 0x0e, 0x47, 0x7b
                },
                0x00000000000e477b)]
    public void ParseUnsignedIntTest(byte[] binaryValue, int expectedValue) =>
        Assert.Equal(expectedValue, BinaryPropertyListParser.ParseUnsignedInt(binaryValue));

    [Theory]
    [InlineData(new byte[]
                {
                    0x57
                },
                0x57)]
    [InlineData(new byte[]
                {
                    0x12, 0x34
                },
                0x1234)]
    [InlineData(new byte[]
                {
                    0x12, 0x34, 0x56
                },
                0x123456)]
    [InlineData(new byte[]
                {
                    0x40, 0x2d, 0xf8, 0x4d
                },
                0x402df84d)]
    [InlineData(new byte[]
                {
                    0x12, 0x34, 0x56, 0x78, 0x9a
                },
                0x123456789a)]
    [InlineData(new byte[]
                {
                    0x12, 0x34, 0x56, 0x78, 0x9a, 0xbc
                },
                0x123456789abc)]
    [InlineData(new byte[]
                {
                    0x12, 0x34, 0x56, 0x78, 0x9a, 0xbc, 0xde
                },
                0x123456789abcde)]
    [InlineData(new byte[]
                {
                    0x41, 0xb4, 0x83, 0x98, 0x2a, 0x00, 0x00, 0x00
                },
                0x41b483982a000000)]
    [InlineData(new byte[]
                {
                    0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0xfc, 0x19
                },
                unchecked((long)0xfffffffffffffc19))]
    [InlineData(new byte[]
                {
                    0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0xfc, 0x19
                },
                unchecked((long)0xfffffffffffffc19))]
    public void ParseLongTest(byte[] binaryValue, long expectedValue) =>
        Assert.Equal(expectedValue, BinaryPropertyListParser.ParseLong(binaryValue));

    [Theory]
    [InlineData(new byte[]
                {
                    0x41, 0xb4, 0x83, 0x98, 0x2a, 0x00, 0x00, 0x00
                },
                344168490)]
    [InlineData(new byte[]
                {
                    0x40, 0x09, 0x21, 0xf9, 0xf0, 0x1b, 0x86, 0x6e
                },
                3.14159)]
    [InlineData(new byte[]
                {
                    0x40, 0x2d, 0xf8, 0x4d
                },
                2.71828007698059)]
    public void ParseDoubleTest(byte[] binaryValue, double expectedValue) =>
        Assert.Equal(expectedValue, BinaryPropertyListParser.ParseDouble(binaryValue), 14);
}