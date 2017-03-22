using Claunia.PropertyList;
using Xunit;
using System.IO;

namespace plistcil.test
{
    public class PropertyListParserTests
    {
        [Fact]
        public static void ParseEmptyStreamTest()
        {
            Assert.Throws<PropertyListFormatException>(() => ParseEmptyStreamTestDelegate());
        }

        static void ParseEmptyStreamTestDelegate()
        {
            using (MemoryStream stream = new MemoryStream())
            {
                PropertyListParser.Parse(stream);
            }
        }
    }
}
