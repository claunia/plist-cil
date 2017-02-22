using Claunia.PropertyList;
using NUnit.Framework;
using System.IO;

namespace plistcil.test
{
    [TestFixture]
    public class PropertyListParserTests
    {
        [Test]
        public static void ParseEmptyStreamTest()
        {
            Assert.Throws<PropertyListFormatException>(new TestDelegate(ParseEmptyStreamTestDelegate));
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
