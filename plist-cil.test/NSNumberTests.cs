using Claunia.PropertyList;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace plistcil.test
{
    [TestFixture]
    public class NSNumberTests
    {
        [Test]
        public static void NSNumberConstructorTest()
        {
            var number = new NSNumber("10032936613", NSNumber.INTEGER);
            Assert.AreEqual(NSNumber.INTEGER, number.GetNSNumberType());
            Assert.AreEqual(10032936613, number.ToObject());
        }

        // The tests below make sure the numbers are being parsed correctly, and do not depend on the culture info
        // being set. Especially, decimal point may vary between cultures and we don't want to take a dependency on that
        // The value being used comes seen in a real property list:
        // <key>TimeZoneOffsetFromUTC</key>
        // <real>7200.000000</real>

        [Test]
        [SetCulture("en-US")]
        public static void ParseNumberEnTest()
        {
            var number = new NSNumber("7200.000001");
            Assert.IsTrue(number.isReal());
            Assert.AreEqual(7200.000001d, number.ToDouble());
        }

        [Test]
        [SetCulture("nl-BE")]
        public static void ParseNumberNlTest()
        {
            // As seen in a real property list:
            // <key>TimeZoneOffsetFromUTC</key>
            // <real>7200.000000</real>
            var number = new NSNumber("7200.000001");
            Assert.IsTrue(number.isReal());
            Assert.AreEqual(7200.000001d, number.ToDouble());
        }

        [Test]
        [SetCulture("en-US")]
        public static void ParseNumberEnTest2()
        {
            // As seen in a real property list:
            // <key>TimeZoneOffsetFromUTC</key>
            // <real>7200.000000</real>
            var number = new NSNumber("7200.000000", NSNumber.REAL);
            Assert.IsTrue(number.isReal());
            Assert.AreEqual(7200d, number.ToDouble());
        }

        [Test]
        [SetCulture("nl-BE")]
        public static void ParseNumberNlTest2()
        {
            // As seen in a real property list:
            // <key>TimeZoneOffsetFromUTC</key>
            // <real>7200.000000</real>
            var number = new NSNumber("7200.000000", NSNumber.REAL);
            Assert.IsTrue(number.isReal());
            Assert.AreEqual(7200d, number.ToDouble());
        }
    }
}
