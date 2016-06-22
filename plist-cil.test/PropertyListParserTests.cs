using Claunia.PropertyList;
using NUnit.Framework;
using System.IO;

namespace plistcil.test
{
    [TestFixture]
    public class PropertyListParserTests
    {
        [Test]
        [ExpectedException(typeof(PropertyListFormatException))]
        public static void ParseEmptyStreamTest()
        {
            using (MemoryStream stream = new MemoryStream())
            {
                PropertyListParser.Parse(stream);
            }
        }
    }
}
