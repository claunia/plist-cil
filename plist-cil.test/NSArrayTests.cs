using Claunia.PropertyList;
using Xunit;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace plistcil.test
{
    public class NSArrayTests
    {
        /// <summary>
        /// Tests the addition of a .NET object to the NSArray
        /// </summary>
        [Fact]
        public void AddAndContainsObjectTest()
        {
            NSArray array = new NSArray();
            array.Add(1);

            Assert.True(array.Contains(1));
            Assert.False(array.Contains(2));
        }

        /// <summary>
        /// Tests the <see cref="NSArray.IndexOf(object)"/> method for .NET objects.
        /// </summary>
        [Fact]
        public void IndexOfTest()
        {
            NSArray array = new NSArray();
            array.Add(1);
            array.Add("test");

            Assert.Equal(0, array.IndexOf(1));
            Assert.Equal(1, array.IndexOf("test"));
        }

        /// <summary>
        /// Tests the <see cref="NSArray.Insert(int, object)"/> method for a 
        /// .NET object.
        /// </summary>
        [Fact]
        public void InsertTest()
        {
            NSArray array = new NSArray();
            array.Add(0);
            array.Add(1);
            array.Add(2);

            array.Insert(1, "test");

            Assert.Equal(4, array.Count);
            Assert.Equal("test", array.ObjectAtIndex(1).ToObject());
        }

        /// <summary>
        /// Tests the <see cref="NSArray.Remove(object)"/> method for a .NET object.
        /// </summary>
        [Fact]
        public void RemoveTest()
        {
            NSArray array = new NSArray();
            array.Add(0);
            Assert.False(array.Remove((object)1));
            Assert.True(array.Remove((object)0));

            Assert.Equal(0, array.Count);
        }

        /// <summary>
        /// Tests the <see cref="NSArray.GetEnumerator"/> method.
        /// </summary>
        [Fact]
        public void EnumeratorTest()
        {
            NSArray array = new NSArray();
            array.Add(0);
            array.Add(1);

            var enumerator = array.GetEnumerator();

            Assert.Null(enumerator.Current);

            Assert.True(enumerator.MoveNext());
            Assert.Equal(new NSNumber(0), enumerator.Current);

            Assert.True(enumerator.MoveNext());
            Assert.Equal(new NSNumber(1), enumerator.Current);

            Assert.False(enumerator.MoveNext());
        }
    }
}
