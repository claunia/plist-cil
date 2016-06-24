using Claunia.PropertyList;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace plistcil.test
{
    [TestFixture]
    public class NSDateTests
    {
        [Test]
        public static void ConstructorTest()
        {
            var actual = new NSDate("2000-01-01T00:00:00Z");
            var expected = new DateTime(2000, 1, 1, 0, 0, 0, DateTimeKind.Utc);
            Assert.AreEqual(expected, actual.Date.ToUniversalTime());
        }

        [Test]
        public static void MakeDateStringTest()
        {
            var date = new DateTime(2000, 1, 1, 0, 0, 0, DateTimeKind.Utc);
            var expected = "2000-01-01T00:00:00Z";
            var actual = NSDate.MakeDateString(date);

            Assert.AreEqual(expected, actual);
        }
    }
}
