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
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Collections;
using System.Text;

namespace Claunia.PropertyList
{
    /// <summary>
    /// A set is an interface to an unordered collection of objects.
    /// This implementation uses a <see cref="List"/>as the underlying
    /// data structure.
    /// </summary>
    /// @author Daniel Dreibrodt
    public class NSSet : NSObject, IEnumerable
    {
        List<NSObject> set;

        // No need for this. It's easier in C# to just follow NSSet unordered as-is
        // private bool ordered = false;

        /// <summary>
        /// Creates an empty unordered set.
        /// </summary>
        public NSSet() {
            set = new List<NSObject>();
        }

        /**
        * Creates an empty set.
        *
        * @param ordered Indicates whether the created set should be ordered or unordered.
        * @see java.util.LinkedHashSet
        * @see java.util.TreeSet
        */
        // No need for this. It's easier in C# to just follow NSSet unordered as-is
        /*
        public NSSet(boolean ordered) {
            this.ordered = ordered;
            if (!ordered)
                set = new LinkedHashSet<NSObject>();
            else
                set = new TreeSet<NSObject>();
        }*/

        /// <summary>
        /// Create a set and fill it with the given objects.
        /// </summary>
        /// <param name="objects">The objects to populate the set.</param>
        public NSSet(params NSObject[] objects) {
            set = new List<NSObject>(objects);
        }

        /**
        * Create a set and fill it with the given objects.
        *
        * @param objects The objects to populate the set.
        * @see java.util.LinkedHashSet
        * @see java.util.TreeSet
        */
        // No need for this. It's easier in C# to just follow NSSet unordered as-is
        /*
        public NSSet(boolean ordered, NSObject... objects) {
            this.ordered = ordered;
            if (!ordered)
                set = new LinkedHashSet<NSObject>();
            else
                set = new TreeSet<NSObject>();
            set.addAll(Arrays.asList(objects));
        }*/

        /// <summary>
        /// Adds an object to the set.
        /// </summary>
        /// <param name="obj">The object to add.</param>
        [MethodImpl(MethodImplOptions.Synchronized)]
        public void AddObject(NSObject obj) {
            set.Add(obj);
        }

        /// <summary>
        /// Removes an object from the set.
        /// </summary>
        /// <param name="obj">The object to remove.</param>
        [MethodImpl(MethodImplOptions.Synchronized)]
        public void RemoveObject(NSObject obj) {
            set.Remove(obj);
        }

        /// <summary>
        /// Returns all objects contained in the set.
        /// </summary>
        /// <returns>An array of all objects in the set.</returns>
        [MethodImpl(MethodImplOptions.Synchronized)]
        public NSObject[] AllObjects() {
            return set.ToArray();
        }

        /// <summary>
        /// Returns one of the objects in the set, or <code>null</code>
        /// if the set contains no objects.
        /// </summary>
        /// <returns>The first object in the set, or <code>null</code> if the set is empty.</returns>
        [MethodImpl(MethodImplOptions.Synchronized)]
        public NSObject AnyObject() {
            if (set.Count == 0)
                return null;
            else
                return set[0];
        }

        /// <summary>
        /// Finds out whether a given object is contained in the set.
        /// </summary>
        /// <returns><c>true</c>, when the object was found, <c>false</c> otherwise.</returns>
        /// <param name="obj">The object to look for.</param>
        public bool ContainsObject(NSObject obj) {
            return set.Contains(obj);
        }

        /// <summary>
        /// Determines whether the set contains an object equal to a given object
        /// and returns that object if it is present.
        /// </summary>
        /// <param name="obj">The object to look for.</param>
        /// <returns>The object if it is present, <code>null</code> otherwise.</returns>
        [MethodImpl(MethodImplOptions.Synchronized)]
        public NSObject Member(NSObject obj) {
            foreach (NSObject o in set) {
                if (o.Equals(obj))
                    return o;
            }
            return null;
        }

        /// <summary>
        /// Finds out whether at least one object is present in both sets.
        /// </summary>
        /// <returns><c>true</c> if the intersection of both sets is empty, <c>false</c> otherwise.</returns>
        /// <param name="otherSet">The other set.</param>
        [MethodImpl(MethodImplOptions.Synchronized)]
        public bool IntersectsSet(NSSet otherSet) {
            foreach (NSObject o in set) {
                if (otherSet.ContainsObject(o))
                    return true;
            }
            return false;
        }

        /// <summary>
        /// Finds out if this set is a subset of the given set.
        /// </summary>
        /// <returns><c>true</c> if all elements in this set are also present in the other set, <c>false</c>otherwise.</returns>
        /// <param name="otherSet">The other set.</param>
        [MethodImpl(MethodImplOptions.Synchronized)]
        public bool IsSubsetOfSet(NSSet otherSet) {
            foreach (NSObject o in set) {
                if (!otherSet.ContainsObject(o))
                    return false;
            }
            return true;
        }

        /// <summary>
        /// Returns an enumerator object that lets you iterate over all elements of the set.
        /// This is the equivalent to <code>objectEnumerator</code> in the Cocoa implementation
        /// of NSSet.
        /// </summary>
        /// <returns>The iterator for the set.</returns>
        [MethodImpl(MethodImplOptions.Synchronized)]
        public IEnumerator GetEnumerator() {
            return set.GetEnumerator();
        }

        /// <summary>
        /// Gets the underlying data structure in which this NSSets stores its content.
        /// </summary>
        /// <returns>A Set object.</returns>
        internal List<NSObject> GetSet() {
            return set;
        }

        public override int GetHashCode() {
            int hash = 7;
            hash = 29 * hash + (this.set != null ? this.set.GetHashCode() : 0);
            return hash;
        }

        public override bool Equals(Object obj) {
            if (obj == null) {
                return false;
            }
            if (GetType() != obj.GetType()) {
                return false;
            }
            NSSet other = (NSSet) obj;
            return !(this.set != other.set && (this.set == null || !this.set.Equals(other.set)));
        }

        /// <summary>
        /// Gets the number of elements in the set.
        /// </summary>
        /// <value>The number of elements in the set.</value>
        public int Count
        {
            get
            {
                lock (set)
                {
                    return set.Count;
                }
            }
        }

        /// <summary>
        /// Returns the XML representantion for this set.
        /// There is no official XML representation specified for sets.
        /// In this implementation it is represented by an array.
        /// </summary>
        /// <param name="xml">The XML StringBuilder</param>
        /// <param name="level">The indentation level</param>
        internal override void ToXml(StringBuilder xml, int level) {
            Indent(xml, level);
            xml.Append("<array>");
            xml.Append(NSObject.NEWLINE);
            foreach (NSObject o in set) {
                o.ToXml(xml, level + 1);
                xml.Append(NSObject.NEWLINE);
            }
            Indent(xml, level);
            xml.Append("</array>");
        }

        // TODO: Implement BinaryPropertyListWriter
        /*
        @Override
        void assignIDs(BinaryPropertyListWriter out) {
            super.assignIDs(out);
            for (NSObject obj : set) {
                obj.assignIDs(out);
            }
        }

        @Override
        void toBinary(BinaryPropertyListWriter out) throws IOException {
            if (ordered) {
                out.writeIntHeader(0xB, set.size());
            } else {
                out.writeIntHeader(0xC, set.size());
            }
            for (NSObject obj : set) {
                out.writeID(out.getID(obj));
            }
        }*/

        /// <summary>
        /// Returns the ASCII representation of this set.
        /// There is no official ASCII representation for sets.
        /// In this implementation sets are represented as arrays.
        /// </summary>
        /// <param name="ascii">The ASCII file string builder</param>
        /// <param name="level">The indentation level</param>
        internal override void ToASCII(StringBuilder ascii, int level) {
            Indent(ascii, level);
            NSObject[] array = AllObjects();
            ascii.Append(ASCIIPropertyListParser.ARRAY_BEGIN_TOKEN);
            int indexOfLastNewLine = ascii.ToString().LastIndexOf(NEWLINE);
            for (int i = 0; i < array.Length; i++) {
                Type objClass = array[i].GetType();
                if ((objClass.Equals(typeof(NSDictionary)) || objClass.Equals(typeof(NSArray)) || objClass.Equals(typeof(NSData)))
                    && indexOfLastNewLine != ascii.Length) {
                    ascii.Append(NEWLINE);
                    indexOfLastNewLine = ascii.Length;
                    array[i].ToASCII(ascii, level + 1);
                } else {
                    if (i != 0)
                        ascii.Append(" ");
                    array[i].ToASCII(ascii, 0);
                }

                if (i != array.Length - 1)
                    ascii.Append(ASCIIPropertyListParser.ARRAY_ITEM_DELIMITER_TOKEN);

                if (ascii.Length - indexOfLastNewLine > ASCII_LINE_LENGTH) {
                    ascii.Append(NEWLINE);
                    indexOfLastNewLine = ascii.Length;
                }
            }
            ascii.Append(ASCIIPropertyListParser.ARRAY_END_TOKEN);
        }

        /// <summary>
        /// Returns the ASCII representation of this set according to the GnuStep format.
        /// There is no official ASCII representation for sets.
        /// In this implementation sets are represented as arrays.
        /// </summary>
        /// <param name="ascii">The ASCII file string builder</param>
        /// <param name="level">The indentation level</param>
        internal override void ToASCIIGnuStep(StringBuilder ascii, int level) {
            Indent(ascii, level);
            NSObject[] array = AllObjects();
            ascii.Append(ASCIIPropertyListParser.ARRAY_BEGIN_TOKEN);
            int indexOfLastNewLine = ascii.ToString().LastIndexOf(NEWLINE);
            for (int i = 0; i < array.Length; i++) {
                Type objClass = array[i].GetType();
                if ((objClass.Equals(typeof(NSDictionary)) || objClass.Equals(typeof(NSArray)) || objClass.Equals(typeof(NSData)))
                    && indexOfLastNewLine != ascii.Length) {
                    ascii.Append(NEWLINE);
                    indexOfLastNewLine = ascii.Length;
                    array[i].ToASCIIGnuStep(ascii, level + 1);
                } else {
                    if (i != 0)
                        ascii.Append(" ");
                    array[i].ToASCIIGnuStep(ascii, 0);
                }

                if (i != array.Length - 1)
                    ascii.Append(ASCIIPropertyListParser.ARRAY_ITEM_DELIMITER_TOKEN);

                if (ascii.Length - indexOfLastNewLine > ASCII_LINE_LENGTH) {
                    ascii.Append(NEWLINE);
                    indexOfLastNewLine = ascii.Length;
                }
            }
            ascii.Append(ASCIIPropertyListParser.ARRAY_END_TOKEN);
        }
    }
}

