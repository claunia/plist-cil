using System.IO;
using Claunia.PropertyList;
using Xunit;

namespace plistcil.test
{
    public class UIDTests
    {
        [Theory]
        [InlineData(new byte[] {0xAB})]
        [InlineData(new byte[] {0xAB, 0xCD})]
        [InlineData(new byte[] {0xAB, 0xCD, 0xEF, 0xFE})]
        [InlineData(new byte[] {0xAB, 0xCD, 0xEF, 0xFE, 0xFE, 0xEF, 0xCD, 0xAB})]
        public void UidFromArrayTest(byte[] array)
        {
            UID uid = new UID(array);
            Assert.Equal(array, uid.Bytes);
        }

        [Fact]
        public void BinaryRoundTripTest()
        {
            UID original = new UID(0xabcd);

            using(MemoryStream stream = new MemoryStream())
            {
                BinaryPropertyListWriter.Write(stream, original);
                stream.Position = 0;
                UID roundtrip = BinaryPropertyListParser.Parse(stream) as UID;
                Assert.Equal(original.Bytes, roundtrip.Bytes);
            }
        }

        [Fact]
        public void ByteUidTest()
        {
            UID uid = new UID(0xAB);
            Assert.Equal(new byte[] {0xAB}, uid.Bytes);
        }

        [Fact]
        public void IntUidTest()
        {
            UID uid = new UID(0xABCDEF00);
            Assert.Equal(new byte[] {0xAB, 0xCD, 0xEF, 0x00}, uid.Bytes);
        }

        [Fact]
        public void LongUidTest()
        {
            UID uid = new UID(0xABCDEF0000EFCDAB);
            Assert.Equal(new byte[] {0xAB, 0xCD, 0xEF, 0x00, 0x00, 0xEF, 0xCD, 0xAB}, uid.Bytes);
        }

        [Fact]
        public void UIntUidTest()
        {
            UID uid = new UID(0xABCDEF00u);
            Assert.Equal(new byte[] {0xAB, 0xCD, 0xEF, 0x00}, uid.Bytes);
        }

        [Fact]
        public void ULongUidTest()
        {
            UID uid = new UID(0xABCDEF0000EFCDABu);
            Assert.Equal(new byte[] {0xAB, 0xCD, 0xEF, 0x00, 0x00, 0xEF, 0xCD, 0xAB}, uid.Bytes);
        }

        [Fact]
        public void UShortUidTest()
        {
            UID uid = new UID(0xABCDu);
            Assert.Equal(new byte[] {0xAB, 0xCD}, uid.Bytes);
        }

        [Fact]
        public void XmlRoundTripTest()
        {
            UID original = new UID(0xabcd);

            string plist = original.ToXmlPropertyList();

            // UIDs don't exist in XML property lists, but they are represented as strings
            // for compability purposes
            NSString roundtrip = XmlPropertyListParser.ParseString(plist) as NSString;
            Assert.Equal("abcd", roundtrip.ToObject());
        }
    }
}