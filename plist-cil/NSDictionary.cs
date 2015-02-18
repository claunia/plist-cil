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
using System.Text;

namespace Claunia.PropertyList
{
    /// <summary>
    /// A NSDictionary is a collection of keys and values, essentially a Dictionary.
    /// The keys are simple Strings whereas the values can be any kind of NSObject.
    ///
    /// You can access the keys through the function <code>AllKeys()</code>. Access
    /// to the objects stored for each key is given through the function
    /// <code>ObjectoForKey(String key)</code>.
    /// </summary>
    public class NSDictionary : NSObject, IDictionary<string, NSObject>
    {
        Dictionary<string, NSObject> dict;

        /// <summary>
        /// Creates a new empty NSDictionary.
        /// </summary>
        public NSDictionary() {
            dict = new Dictionary<string, NSObject>();
        }

        /// <summary>
        /// Gets the hashmap which stores the keys and values of this dictionary.
        /// Changes to the hashmap's contents are directly reflected in this
        /// dictionary.
        /// </summary>
        /// <returns>The hashmap which is used by this dictionary to store its contents.</returns>
        public Dictionary<string, NSObject> GetDictionary() {
            return dict;
        }

        /// <summary>
        /// Gets the NSObject stored for the given key.
        /// </summary>
        /// <returns>The object.</returns>
        /// <param name="key">The key.</param>
        public NSObject ObjectForKey(string key) {
            NSObject nso;
            return dict.TryGetValue(key, out nso) ? nso : null;
        }

        public bool IsEmpty
        {
            get {
                return dict.Count == 0;
            }
        }

        public bool ContainsKey(Object key)
        {
            return key is string && dict.ContainsKey((string)key);
        }

        public bool Remove(Object key)
        {
            return key is string && dict.Remove((string)key);
        }

        public NSObject Get(Object key)
        {
            if(key is string)
                return ObjectForKey((string)key);
            return null;
        }

        // TODO: Implement NSObject.Wrap(Object)
        /*
        public boolean containsValue(Object value) {
            if(value == null)
                return false;
            NSObject wrap = NSObject.wrap(value);
            return dict.containsValue(wrap);
        }*

        **
        * Puts a new key-value pair into this dictionary.
        * If the value is null, no operation will be performed on the dictionary.
        *
        * @param key The key.
        * @param obj The value. Supported object types are numbers, byte-arrays, dates, strings and arrays or sets of those.
        * @return The value previously associated to the given key,
            *         or null, if no value was associated to it.
                *
                public void Add(String key, Object obj) {
                if(obj == null)
                    return dict.get(key);
                return Add(key, NSObject.wrap(obj));
            }*/

        /// <summary>
        /// Puts a new key-value pair into this dictionary.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <param name="obj">The value.</param>
        public void Add(string key, long obj) {
            Add(key, new NSNumber(obj));
        }

        /// <summary>
        /// Puts a new key-value pair into this dictionary.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <param name="obj">The value.</param>
        public void Add(string key, double obj) {
            Add(key, new NSNumber(obj));
        }

        /// <summary>
        /// Puts a new key-value pair into this dictionary.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <param name="obj">The value.</param>
        public void Add(string key, bool obj) {
            Add(key, new NSNumber(obj));
        }

        /// <summary>
        /// Checks whether a given value is contained in this dictionary.
        /// </summary>
        /// <param name="val">The value that will be searched for.</param>
        /// <returns>Whether the key is contained in this dictionary.</returns>
        public bool ContainsValue(string val) {
            foreach (NSObject o in dict.Values) {
                if (o.GetType().Equals(typeof(NSString))) {
                    NSString str = (NSString) o;
                    if (str.GetContent().Equals(val))
                        return true;
                }
            }
            return false;
        }

        /// <summary>
        /// Checks whether a given value is contained in this dictionary.
        /// </summary>
        /// <param name="val">The value that will be searched for.</param>
        /// <returns>Whether the key is contained in this dictionary.</returns>
        public bool ContainsValue(long val) {
            foreach (NSObject o in dict.Values) {
                if (o.GetType().Equals(typeof(NSNumber))) {
                    NSNumber num = (NSNumber) o;
                    if (num.isInteger() && num.ToInt() == val)
                        return true;
                }
            }
            return false;
        }

        /// <summary>
        /// Checks whether a given value is contained in this dictionary.
        /// </summary>
        /// <param name="val">The value that will be searched for.</param>
        /// <returns>Whether the key is contained in this dictionary.</returns>
        public bool ContainsValue(double val) {
            foreach (NSObject o in dict.Values) {
                if (o.GetType().Equals(typeof(NSNumber))) {
                    NSNumber num = (NSNumber) o;
                    if (num.isReal() && num.ToDouble() == val)
                        return true;
                }
            }
            return false;
        }

        /// <summary>
        /// Checks whether a given value is contained in this dictionary.
        /// </summary>
        /// <param name="val">The value that will be searched for.</param>
        /// <returns>Whether the key is contained in this dictionary.</returns>
        public bool ContainsValue(bool val) {
            foreach (NSObject o in dict.Values) {
                if (o.GetType().Equals(typeof(NSNumber))) {
                    NSNumber num = (NSNumber) o;
                    if (num.isBoolean() && num.ToBool() == val)
                        return true;
                }
            }
            return false;
        }

        /// <summary>
        /// Checks whether a given value is contained in this dictionary.
        /// </summary>
        /// <param name="val">The value that will be searched for.</param>
        /// <returns>Whether the key is contained in this dictionary.</returns>
        public bool ContainsValue(DateTime val) {
            foreach (NSObject o in dict.Values) {
                if (o.GetType().Equals(typeof(NSDate))) {
                    NSDate dat = (NSDate) o;
                    if (dat.Date.Equals(val))
                        return true;
                }
            }
            return false;
        }

        /// <summary>
        /// Checks whether a given value is contained in this dictionary.
        /// </summary>
        /// <param name="val">The value that will be searched for.</param>
        /// <returns>Whether the key is contained in this dictionary.</returns>
        public bool ContainsValue(byte[] val) {
            foreach (NSObject o in dict.Values) {
                if (o.GetType().Equals(typeof(NSData))) {
                    NSData dat = (NSData) o;
                    if (Array.Equals(dat.Bytes, val))
                        return true;
                }
            }
            return false;
        }

        public override bool Equals(Object obj) {
            return (obj.GetType().Equals(GetType()) && ((NSDictionary) obj).dict.Equals(dict));
        }

        public override int GetHashCode() {
            int hash = 7;
            hash = 83 * hash + (this.dict != null ? this.dict.GetHashCode() : 0);
            return hash;
        }

        internal override void ToXml(StringBuilder xml, int level) {
            Indent(xml, level);
            xml.Append("<dict>");
            xml.Append(NSObject.NEWLINE);
            foreach(KeyValuePair<string, NSObject> kvp in dict) {
                Indent(xml, level + 1);
                xml.Append("<key>");
                //According to http://www.w3.org/TR/REC-xml/#syntax node values must not
                //contain the characters < or &. Also the > character should be escaped.
                if (kvp.Key.Contains("&") || kvp.Key.Contains("<") || kvp.Key.Contains(">")) {
                    xml.Append("<![CDATA[");
                    xml.Append(kvp.Key.Replace("]]>", "]]]]><![CDATA[>"));
                    xml.Append("]]>");
                } else {
                    xml.Append(kvp.Key);
                }
                xml.Append("</key>");
                xml.Append(NSObject.NEWLINE);
                kvp.Value.ToXml(xml, level + 1);
                xml.Append(NSObject.NEWLINE);
            }
            Indent(xml, level);
            xml.Append("</dict>");
        }

        // TODO: BinaryPropertyListWriter
        /*
        override void assignIDs(BinaryPropertyListWriter out) {
            super.assignIDs(out);
            for (Map.Entry<String, NSObject> entry : dict.entrySet()) {
                new NSString(entry.getKey()).assignIDs(out);
                entry.getValue().assignIDs(out);
            }
        }

        override void toBinary(BinaryPropertyListWriter out) throws IOException {
            out.writeIntHeader(0xD, dict.size());
            Set<Map.Entry<String, NSObject>> entries = dict.entrySet();
            for (Map.Entry<String, NSObject> entry : entries) {
                out.writeID(out.getID(new NSString(entry.getKey())));
            }
            for (Map.Entry<String, NSObject> entry : entries) {
                out.writeID(out.getID(entry.getValue()));
            }
        }*/

        /// <summary>
        /// Generates a valid ASCII property list which has this NSDictionary as its
        /// root object. The generated property list complies with the format as
        /// described in <a href="https://developer.apple.com/library/mac/#documentation/Cocoa/Conceptual/PropertyLists/OldStylePlists/OldStylePLists.html">
        /// Property List Programming Guide - Old-Style ASCII Property Lists</a>.
        /// </summary>
        /// <returns>ASCII representation of this object.</returns>
        public string ToASCIIPropertyList() {
            StringBuilder ascii = new StringBuilder();
            ToASCII(ascii, 0);
            ascii.Append(NEWLINE);
            return ascii.ToString();
        }

        /// <summary>
        /// Generates a valid ASCII property list in GnuStep format which has this
        /// NSDictionary as its root object. The generated property list complies with
        /// the format as described in <a href="http://www.gnustep.org/resources/documentation/Developer/Base/Reference/NSPropertyList.html">
        /// GnuStep - NSPropertyListSerialization class documentation</a>
        /// </summary>
        /// <returns>GnuStep ASCII representation of this object.</returns>
        public string ToGnuStepASCIIPropertyList() {
            StringBuilder ascii = new StringBuilder();
            ToASCIIGnuStep(ascii, 0);
            ascii.Append(NEWLINE);
            return ascii.ToString();
        }

        protected override void ToASCII(StringBuilder ascii, int level) {
            // TODO: Implement ASCIIPropertyListParser
            /*
            indent(ascii, level);
            ascii.append(ASCIIPropertyListParser.DICTIONARY_BEGIN_TOKEN);
            ascii.append(NEWLINE);
            String[] keys = allKeys();
            for (String key : keys) {
                NSObject val = objectForKey(key);
                indent(ascii, level + 1);
                ascii.append("\"");
                ascii.append(NSString.escapeStringForASCII(key));
                ascii.append("\" =");
                Class<?> objClass = val.getClass();
                if (objClass.equals(NSDictionary.class) || objClass.equals(NSArray.class) || objClass.equals(NSData.class)) {
                    ascii.append(NEWLINE);
                    val.toASCII(ascii, level + 2);
                } else {
                    ascii.append(" ");
                    val.toASCII(ascii, 0);
                }
                ascii.append(ASCIIPropertyListParser.DICTIONARY_ITEM_DELIMITER_TOKEN);
                ascii.append(NEWLINE);
            }
            indent(ascii, level);
            ascii.append(ASCIIPropertyListParser.DICTIONARY_END_TOKEN);*/
        }

        protected override void ToASCIIGnuStep(StringBuilder ascii, int level) {
            // TODO: Implement ASCIIPropertyListParser
            /*
            indent(ascii, level);
            ascii.append(ASCIIPropertyListParser.DICTIONARY_BEGIN_TOKEN);
            ascii.append(NEWLINE);
            String[] keys = dict.keySet().toArray(new String[dict.size()]);
            for (String key : keys) {
                NSObject val = objectForKey(key);
                indent(ascii, level + 1);
                ascii.append("\"");
                ascii.append(NSString.escapeStringForASCII(key));
                ascii.append("\" =");
                Class<?> objClass = val.getClass();
                if (objClass.equals(NSDictionary.class) || objClass.equals(NSArray.class) || objClass.equals(NSData.class)) {
                    ascii.append(NEWLINE);
                    val.toASCIIGnuStep(ascii, level + 2);
                } else {
                    ascii.append(" ");
                    val.toASCIIGnuStep(ascii, 0);
                }
                ascii.append(ASCIIPropertyListParser.DICTIONARY_ITEM_DELIMITER_TOKEN);
                ascii.append(NEWLINE);
            }
            indent(ascii, level);
            ascii.append(ASCIIPropertyListParser.DICTIONARY_END_TOKEN);*/
        }

        #region IDictionary implementation
        public void Add(string key, NSObject value)
        {
            dict.Add(key, value);
        }

        public bool ContainsKey(string key)
        {
            return dict.ContainsKey(key);
        }

        public bool ContainsValue(NSObject key)
        {
            return dict.ContainsValue(key);
        }

        public bool Remove(string key)
        {
            return dict.Remove(key);
        }

        public bool TryGetValue(string key, out NSObject value)
        {
            return dict.TryGetValue(key, out value);
        }

        public NSObject this[string index]
        {
            get
            {
                return dict[index];
            }
            set
            {
                dict[index] = value;
            }
        }
        public ICollection<string> Keys
        {
            get
            {
                return dict.Keys;
            }
        }
        public ICollection<NSObject> Values
        {
            get
            {
                return dict.Values;
            }
        }
        #endregion
        #region ICollection implementation
        public void Add(KeyValuePair<string, NSObject> item)
        {
            dict.Add(item.Key, item.Value);
        }
        public void Clear()
        {
            dict.Clear();
        }
        public bool Contains(KeyValuePair<string, NSObject> item)
        {
            return dict.ContainsKey(item.Key);
        }
        public void CopyTo(KeyValuePair<string, NSObject>[] array, int arrayIndex)
        {
            throw new NotImplementedException();
        }
        public bool Remove(KeyValuePair<string, NSObject> item)
        {
            return dict.Remove(item.Key);
        }
        public int Count
        {
            get
            {
                return dict.Count;
            }
        }
        public bool IsReadOnly
        {
            get
            {
                return false;
            }
        }
        #endregion
        #region IEnumerable implementation
        public IEnumerator<KeyValuePair<string, NSObject>> GetEnumerator()
        {
            return dict.GetEnumerator();
        }
        #endregion
        #region IEnumerable implementation
        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return dict.GetEnumerator();
        }
        #endregion
    }
}

