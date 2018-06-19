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
            var uid = new UID((byte)0xAB);
            Assert.Equal(new byte[] { 0xAB }, uid.Bytes);
        }

        [Fact]
        public void SByteUidTest()
        {
            var uid = new UID("test", unchecked((sbyte)0x0F));
            Assert.Equal(new byte[] { 0x0F }, uid.Bytes);
        }

        [Fact]
        public void ShortUidTest()
        {
            var uid = new UID("test", unchecked((short)0x0F0F));
            Assert.Equal(new byte[] { 0x0F, 0x0F }, uid.Bytes);
        }

        [Fact]
        public void UShortUidTest()
        {
            var uid = new UID(0xABCDu);
            Assert.Equal(new byte[] { 0xAB, 0xCD }, uid.Bytes);
        }

        [Fact]
        public void UIntUidTest()
        {
            var uid = new UID(0xABCDEF00u);
            Assert.Equal(new byte[] { 0xAB, 0xCD, 0xEF, 0x00 }, uid.Bytes);
        }

        [Fact]
        public void IntUidTest()
        {
            var uid = new UID(0xABCDEF00);
            Assert.Equal(new byte[] { 0xAB, 0xCD, 0xEF, 0x00 }, uid.Bytes);
        }

        [Fact]
        public void ULongUidTest()
        {
            var uid = new UID(0xABCDEF0000EFCDABu);
            Assert.Equal(new byte[] { 0xAB, 0xCD, 0xEF, 0x00, 0x00, 0xEF, 0xCD, 0xAB }, uid.Bytes);
        }

        [Fact]
        public void LongUidTest()
        {
            var uid = new UID(0xABCDEF0000EFCDAB);
            Assert.Equal(new byte[] { 0xAB, 0xCD, 0xEF, 0x00, 0x00, 0xEF, 0xCD, 0xAB }, uid.Bytes);
        }

        [Theory]
        [InlineData(new byte[] { 0xAB })]
        [InlineData(new byte[] { 0xAB, 0xCD })]
        [InlineData(new byte[] { 0xAB, 0xCD, 0xEF, 0xFE })]
        [InlineData(new byte[] { 0xAB, 0xCD, 0xEF, 0xFE, 0xFE, 0xEF, 0xCD, 0xAB })]
        public void UidFromArrayTest(byte[] array)
        {
            var uid = new UID(array);
            Assert.Equal(array, uid.Bytes);
        }

        [Fact]
        public void BinaryRoundTripTest()
        {
            var original = new UID(0xabcd);

            using (MemoryStream stream = new MemoryStream())
            {
                BinaryPropertyListWriter.Write(stream, original);
                stream.Position = 0;
                var roundtrip = BinaryPropertyListParser.Parse(stream) as UID;
                Assert.Equal(original.Bytes, roundtrip.Bytes);
            }
        }

        [Fact]
        public void XmlRoundTripTest()
        {
            var original = new UID(0xabcd);

            var plist = original.ToXmlPropertyList();

            // UIDs don't exist in XML property lists, but they are represented as strings
            // for compability purposes
            var roundtrip = XmlPropertyListParser.ParseString(plist) as NSString;
            Assert.Equal("abcd", roundtrip.ToObject());
        }
    }
}
