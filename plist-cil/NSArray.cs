// plist-cil - An open source library to parse and generate property lists for .NET
// Copyright (C) 2015 Natalia Portillo
//
// This code is based on:
// plist - An open source library to parse and generate property lists
// Copyright (C) 2014 Daniel Dreibrodt
//
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
//
// The above copyright notice and this permission notice shall be included in
// all copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
// SOFTWARE.
using System;
using System.Text;

namespace Claunia.PropertyList
{
    /// <summary>
    /// Represents an Array.
    /// </summary>
    /// @author Daniel Dreibrodt
    public class NSArray : NSObject
    {
        NSObject[] array;

        /// <summary>
        /// Creates an empty array of the given length.
        /// </summary>
        /// <param name="length">The number of elements this array will be able to hold.</param>
        public NSArray(int length)
        {
            array = new NSObject[length];
        }

        /// <summary>
        /// Creates a array from an existing one
        /// </summary>
        /// <param name="a">The array which should be wrapped by the NSArray</param>
        public NSArray(params NSObject[] a)
        {
            array = a;
        }

        /// <summary>
        /// Returns the object stored at the given index.
        /// </summary>
        /// <returns>The object at the given index.</returns>
        /// <param name="i">The index of the object.</param>
        public NSObject ObjectAtIndex(int i)
        {
            return array[i];
        }

        /// <summary>
        /// Remove the i-th element from the array.
        /// The array will be resized.
        /// </summary>
        /// <param name="i">The index of the object</param>
        public void Remove(int i)
        {
            if ((i >= array.Length) || (i < 0))
                throw new IndexOutOfRangeException("invalid index:" + i + ";the array length is " + array.Length);
            NSObject[] newArray = new NSObject[array.Length - 1];
            Array.Copy(array, 0, newArray, 0, i);
            Array.Copy(array, i + 1, newArray, i, array.Length - i - 1);
            array = newArray;
        }

        /// <summary>
        /// Stores an object at the specified index.
        /// If there was another object stored at that index it will be replaced.
        /// </summary>
        /// <param name="key">The index where to store the object.</param>
        /// <param name="value">The object.</param>
        public void SetValue(int key, Object value)
        {
            if (value == null)
                throw new ArgumentNullException("Cannot add null values to an NSArray!");
            array[key] = NSObject.Wrap(value);
        }

        /// <summary>
        /// Returns the array of NSObjects represented by this NSArray.
        /// Any changes to the values of this array will also affect the NSArray.
        /// </summary>
        /// <returns>The actual array represented by this NSArray.</returns>
        public NSObject[] GetArray()
        {
            return array;
        }

        /// <summary>
        /// Returns the size of the array.
        /// </summary>
        /// <value>The number of elements that this array can store.</value>
        public int Count
        {
            get
            {
                return array.Length;
            }
        }

        /// <summary>
        /// Checks whether an object is present in the array or whether it is equal
        /// to any of the objects in the array.
        /// </summary>
        /// <returns><c>true</c>, when the object could be found. <c>false</c> otherwise.</returns>
        /// <param name="obj">The object to look for.</param>
        public bool ContainsObject(Object obj)
        {
            NSObject nso = NSObject.Wrap(obj);
            foreach (NSObject elem in array)
            {
                if (elem.Equals(nso))
                {
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// Searches for an object in the array. If it is found its index will be
        /// returned. This method also returns an index if the object is not the same
        /// as the one stored in the array but has equal contents.
        /// </summary>
        /// <returns>The index of the object, if it was found. -1 otherwise.</returns>
        /// <param name="obj">The object to look for.</param>
        public int IndexOfObject(Object obj)
        {
            NSObject nso = NSObject.Wrap(obj);
            for (int i = 0; i < array.Length; i++)
            {
                if (array[i].Equals(nso))
                {
                    return i;
                }
            }
            return -1;
        }

        /// <summary>
        /// Searches for an object in the array. If it is found its index will be
        /// returned. This method only returns the index of an object that is
        /// <b>identical</b> to the given one. Thus objects that might contain the
        /// same value as the given one will not be considered.
        /// </summary>
        /// <returns>The index of the object, if it was found. -1 otherwise.</returns>
        /// <param name="obj">The object to look for.</param>
        public int IndexOfIdenticalObject(Object obj)
        {
            NSObject nso = NSObject.Wrap(obj);
            for (int i = 0; i < array.Length; i++)
            {
                if (array[i] == nso)
                {
                    return i;
                }
            }
            return -1;
        }

        /// <summary>
        /// Returns the last object contained in this array.
        /// </summary>
        /// <returns>The value of the highest index in the array.</returns>
        public NSObject LastObject()
        {
            return array[array.Length - 1];
        }

        /// <summary>
        /// Returns a new array containing only the values stored at the given
        /// indices. The values are sorted by their index.
        /// </summary>
        /// <returns>The new array containing the objects stored at the given indices.</returns>
        /// <param name="indexes">The indices of the objects.</param>
        public NSObject[] objectsAtIndexes(params int[] indexes)
        {
            NSObject[] result = new NSObject[indexes.Length];
            Array.Sort(indexes);
            for (int i = 0; i < indexes.Length; i++)
                result[i] = array[indexes[i]];
            return result;
        }

        public override bool Equals(Object obj)
        {
            if (obj.GetType().Equals(typeof(NSArray)))
            {
                return ArrayEquals(((NSArray)obj).GetArray(), this.array);
            }
            else
            {
                NSObject nso = NSObject.Wrap(obj);
                if (nso.GetType().Equals(typeof(NSArray)))
                {
                    return ArrayEquals(((NSArray)nso).GetArray(), this.array);
                }
            }
            return false;
        }

        public override int GetHashCode()
        {
            int hash = 7;
            hash = 89 * hash + array.GetHashCode();
            return hash;
        }

        internal override void ToXml(StringBuilder xml, int level)
        {
            Indent(xml, level);
            xml.Append("<array>");
            xml.Append(NSObject.NEWLINE);
            foreach (NSObject o in array)
            {
                o.ToXml(xml, level + 1);
                xml.Append(NSObject.NEWLINE);
            }
            Indent(xml, level);
            xml.Append("</array>");
        }

        internal override void AssignIDs(BinaryPropertyListWriter outPlist) {
            base.AssignIDs(outPlist);
            foreach (NSObject obj in array) {
                obj.AssignIDs(outPlist);
            }
        }

        internal override void ToBinary(BinaryPropertyListWriter outPlist) {
            outPlist.WriteIntHeader(0xA, array.Length);
            foreach (NSObject obj in array) {
                outPlist.WriteID(outPlist.GetID(obj));
            }
        }

        /// <summary>
        /// Generates a valid ASCII property list which has this NSArray as its
        /// root object. The generated property list complies with the format as
        /// described in <a href="https://developer.apple.com/library/mac/#documentation/Cocoa/Conceptual/PropertyLists/OldStylePlists/OldStylePLists.html">
        /// Property List Programming Guide - Old-Style ASCII Property Lists</a>.
        /// </summary>
        /// <returns>ASCII representation of this object.</returns>
        public string ToASCIIPropertyList()
        {
            StringBuilder ascii = new StringBuilder();
            ToASCII(ascii, 0);
            ascii.Append(NEWLINE);
            return ascii.ToString();
        }

        /// <summary>
        /// Generates a valid ASCII property list in GnuStep format which has this
        /// NSArray as its root object. The generated property list complies with
        /// the format as described in <a href="http://www.gnustep.org/resources/documentation/Developer/Base/Reference/NSPropertyList.html">
        /// GnuStep - NSPropertyListSerialization class documentation
        /// </a>
        /// </summary>
        /// <returns>GnuStep ASCII representation of this object.</returns>
        public string ToGnuStepASCIIPropertyList()
        {
            StringBuilder ascii = new StringBuilder();
            ToASCIIGnuStep(ascii, 0);
            ascii.Append(NEWLINE);
            return ascii.ToString();
        }

        internal override void ToASCII(StringBuilder ascii, int level)
        {
            Indent(ascii, level);
            ascii.Append(ASCIIPropertyListParser.ARRAY_BEGIN_TOKEN);
            int indexOfLastNewLine = ascii.ToString().LastIndexOf(NEWLINE);
            for (int i = 0; i < array.Length; i++)
            {
                Type objClass = array[i].GetType();
                if ((objClass.Equals(typeof(NSDictionary)) || objClass.Equals(typeof(NSArray)) || objClass.Equals(typeof(NSData)))
                        && indexOfLastNewLine != ascii.Length)
                {
                    ascii.Append(NEWLINE);
                    indexOfLastNewLine = ascii.Length;
                    array[i].ToASCII(ascii, level + 1);
                }
                else
                {
                    if (i != 0)
                        ascii.Append(" ");
                    array[i].ToASCII(ascii, 0);
                }

                if (i != array.Length - 1)
                    ascii.Append(ASCIIPropertyListParser.ARRAY_ITEM_DELIMITER_TOKEN);

                if (ascii.Length - indexOfLastNewLine > ASCII_LINE_LENGTH)
                {
                    ascii.Append(NEWLINE);
                    indexOfLastNewLine = ascii.Length;
                }
            }
            ascii.Append(ASCIIPropertyListParser.ARRAY_END_TOKEN);
        }

        internal override void ToASCIIGnuStep(StringBuilder ascii, int level)
        {
            Indent(ascii, level);
            ascii.Append(ASCIIPropertyListParser.ARRAY_BEGIN_TOKEN);
            int indexOfLastNewLine = ascii.ToString().LastIndexOf(NEWLINE);
            for (int i = 0; i < array.Length; i++)
            {
                Type objClass = array[i].GetType();
                if ((objClass.Equals(typeof(NSDictionary)) || objClass.Equals(typeof(NSArray)) || objClass.Equals(typeof(NSData)))
                        && indexOfLastNewLine != ascii.Length)
                {
                    ascii.Append(NEWLINE);
                    indexOfLastNewLine = ascii.Length;
                    array[i].ToASCIIGnuStep(ascii, level + 1);
                }
                else
                {
                    if (i != 0)
                        ascii.Append(" ");
                    array[i].ToASCIIGnuStep(ascii, 0);
                }

                if (i != array.Length - 1)
                    ascii.Append(ASCIIPropertyListParser.ARRAY_ITEM_DELIMITER_TOKEN);

                if (ascii.Length - indexOfLastNewLine > ASCII_LINE_LENGTH)
                {
                    ascii.Append(NEWLINE);
                    indexOfLastNewLine = ascii.Length;
                }
            }
            ascii.Append(ASCIIPropertyListParser.ARRAY_END_TOKEN);
        }
    }
}

