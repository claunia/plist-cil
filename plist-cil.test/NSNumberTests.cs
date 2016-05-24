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
    public class NSNumberTests
    {
        [Test]
        public static void NSNumberConstructorTest()
        {
            var number = new NSNumber("10032936613", NSNumber.INTEGER);
            Assert.AreEqual(NSNumber.INTEGER, number.GetNSNumberType());
            Assert.AreEqual(10032936613, number.ToObject());
        }
    }
}
