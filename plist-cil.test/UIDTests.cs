using Claunia.PropertyList;
using System.IO;
using Xunit;

namespace plistcil.test
{
    public class UIDTests
    {
        [Fact]
        public void ByteUidTest()
        {
            var uid = new UID("byte", (byte)0xAB);
            Assert.Equal(new byte[] { 0xAB }, uid.Bytes);
            Assert.Equal("byte", uid.Name);
        }

        [Fact]
        public void SByteUidTest()
        {
            var uid = new UID("sbyte", unchecked((sbyte)0xAB));
            Assert.Equal(new byte[] { 0xAB }, uid.Bytes);
            Assert.Equal("sbyte", uid.Name);
        }

        [Fact]
        public void ShortUidTest()
        {
            var uid = new UID("short", unchecked((short)0xABCD));
            Assert.Equal(new byte[] { 0xAB, 0xCD }, uid.Bytes);
            Assert.Equal("short", uid.Name);
        }

        [Fact]
        public void UShortUidTest()
        {
            var uid = new UID("ushort", 0xABCDu);
            Assert.Equal(new byte[] { 0xAB, 0xCD }, uid.Bytes);
            Assert.Equal("ushort", uid.Name);
        }

        [Fact]
        public void UIntUidTest()
        {
            var uid = new UID("uint", 0xABCDEF00u);
            Assert.Equal(new byte[] { 0xAB, 0xCD, 0xEF, 0x00 }, uid.Bytes);
            Assert.Equal("uint", uid.Name);
        }

        [Fact]
        public void IntUidTest()
        {
            var uid = new UID("int", 0xABCDEF00);
            Assert.Equal(new byte[] { 0xAB, 0xCD, 0xEF, 0x00 }, uid.Bytes);
            Assert.Equal("int", uid.Name);
        }

        [Fact]
        public void ULongUidTest()
        {
            var uid = new UID("ulong", 0xABCDEF0000EFCDABu);
            Assert.Equal(new byte[] { 0xAB, 0xCD, 0xEF, 0x00, 0x00, 0xEF, 0xCD, 0xAB }, uid.Bytes);
            Assert.Equal("ulong", uid.Name);
        }

        [Fact]
        public void LongUidTest()
        {
            var uid = new UID("int", 0xABCDEF0000EFCDAB);
            Assert.Equal(new byte[] { 0xAB, 0xCD, 0xEF, 0x00, 0x00, 0xEF, 0xCD, 0xAB }, uid.Bytes);
            Assert.Equal("int", uid.Name);
        }

        [Theory]
        [InlineData(new byte[] { 0xAB })]
        [InlineData(new byte[] { 0xAB, 0xCD })]
        [InlineData(new byte[] { 0xAB, 0xCD, 0xEF, 0xFE })]
        [InlineData(new byte[] { 0xAB, 0xCD, 0xEF, 0xFE, 0xFE, 0xEF, 0xCD, 0xAB })]
        [InlineData(new byte[] { 0x00 })]
        [InlineData(new byte[] { 0x00, 0x00 })]
        [InlineData(new byte[] { 0x00, 0xCD })]
        [InlineData(new byte[] { 0x00, 0x00, 0x00, 0x00 })]
        [InlineData(new byte[] { 0x00, 0x00, 0x00, 0xCD })]
        [InlineData(new byte[] { 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00 })]
        [InlineData(new byte[] { 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0xCD })]
        public void UidFromArrayTest(byte[] array)
        {
            var uid = new UID("array", array);
            Assert.Equal(array, uid.Bytes);
            Assert.Equal("array", uid.Name);
        }

        [Fact]
        public void BinaryRoundTripTest()
        {
            var original = new UID("0", 0xabcd);

            using (MemoryStream stream = new MemoryStream())
            {
                BinaryPropertyListWriter.Write(stream, original);
                stream.Position = 0;
                var roundtrip = BinaryPropertyListParser.Parse(stream) as UID;
                Assert.Equal(original, roundtrip);
            }
        }

        [Fact]
        public void XmlRoundTripTest()
        {
            var original = new UID("0", 0xabcd);

            var plist = original.ToXmlPropertyList();

            // UIDs don't exist in XML property lists, but they are represented as strings
            // for compability purposes
            var roundtrip = XmlPropertyListParser.ParseString(plist) as NSString;
            Assert.Equal("0000abcd", roundtrip.ToObject());
        }
    }
}
