using System.IO;
using Claunia.PropertyList;
using Xunit;

namespace plistcil.test
{
    public class PropertyListParserTests
    {
        static void ParseEmptyStreamTestDelegate()
        {
            using var stream = new MemoryStream();

            PropertyListParser.Parse(stream);
        }

        [Fact]
        public static void ParseEmptyStreamTest() =>
            Assert.Throws<PropertyListFormatException>(ParseEmptyStreamTestDelegate);
    }
}