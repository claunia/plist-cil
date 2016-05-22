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
    public class NSArrayTests
    {
        /// <summary>
        /// Tests the addition of a .NET object to the NSArray
        /// </summary>
        [Test]
        public void AddAndContainsObjectTest()
        {
            NSArray array = new NSArray();
            array.Add(1);

            Assert.IsTrue(array.Contains(1));
            Assert.IsFalse(array.Contains(2));
        }

        /// <summary>
        /// Tests the <see cref="NSArray.IndexOf(object)"/> method for .NET objects.
        /// </summary>
        [Test]
        public void IndexOfTest()
        {
            NSArray array = new NSArray();
            array.Add(1);
            array.Add("test");

            Assert.AreEqual(0, array.IndexOf(1));
            Assert.AreEqual(1, array.IndexOf("test"));
        }

        /// <summary>
        /// Tests the <see cref="NSArray.Insert(int, object)"/> method for a 
        /// .NET object.
        /// </summary>
        [Test]
        public void InsertTest()
        {
            NSArray array = new NSArray();
            array.Add(0);
            array.Add(1);
            array.Add(2);

            array.Insert(1, "test");

            Assert.AreEqual(4, array.Count);
            Assert.AreEqual("test", array.ObjectAtIndex(1).ToObject());
        }

        /// <summary>
        /// Tests the <see cref="NSArray.Remove(object)"/> method for a .NET object.
        /// </summary>
        [Test]
        public void RemoveTest()
        {
            NSArray array = new NSArray();
            array.Add(0);
            Assert.IsFalse(array.Remove((object)1));
            Assert.IsTrue(array.Remove((object)0));

            Assert.AreEqual(0, array.Count);
        }
    }
}
