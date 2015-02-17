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
using System.Collections.Generic;

namespace Claunia.PropertyList
{
    /// <summary>
    /// Abstract interface for any object contained in a property list.
    /// The names and functions of the various objects orient themselves
    /// towards Apple's Cocoa API.
    /// </summary>
    /// @author Daniel Dreibrodt
    public abstract class NSObject
    {
        /// <summary>
        /// The newline character used for generating the XML output.
        /// This constant will be different depending on the operating system on
        /// which you use this library.
        /// </summary>
        readonly static string NEWLINE = Environment.NewLine;


        /// <summary>
        /// The identation character used for generating the XML output. This is the
        /// tabulator character.
        /// </summary>
        readonly static string INDENT = "\t";

        /// <summary>
        /// The maximum length of the text lines to be used when generating
        /// ASCII property lists. But this number is only a guideline it is not
        /// guaranteed that it will not be overstepped.
        /// </summary>
        readonly static int ASCII_LINE_LENGTH = 80;

        /// <summary>
        /// Generates the XML representation of the object (without XML headers or enclosing plist-tags).
        /// </summary>
        /// <param name="xml">The StringBuilder onto which the XML representation is appended.</param>
        /// <param name="level">The indentation level of the object.</param>
        internal abstract void ToXml(StringBuilder xml, int level);

        /// <summary>
        /// Assigns IDs to all the objects in this NSObject subtree.
        /// </summary>
        /// <param name="out">The writer object that handles the binary serialization.</param>
        // TODO: Port BinaryPropertyListWriter class
        /*
        void assignIDs(BinaryPropertyListWriter out) {
            out.assignID(this);
        }*/

        /// <summary>
        /// Generates the binary representation of the object.
        /// </summary>
        /// <param name="out">The output stream to serialize the object to.</param>
        // TODO: Port BinaryPropertyListWriter class
        //abstract void toBinary(BinaryPropertyListWriter out);

        /// <summary>
        /// Generates a valid XML property list including headers using this object as root.
        /// </summary>
        /// <returns>The XML representation of the property list including XML header and doctype information.</returns>
        public string ToXmlPropertyList() {
            StringBuilder xml = new StringBuilder("<?xml version=\"1.0\" encoding=\"UTF-8\"?>");
            xml.Append(NSObject.NEWLINE);
            xml.Append("<!DOCTYPE plist PUBLIC \"-//Apple//DTD PLIST 1.0//EN\" \"http://www.apple.com/DTDs/PropertyList-1.0.dtd\">");
            xml.Append(NSObject.NEWLINE);
            xml.Append("<plist version=\"1.0\">");
            xml.Append(NSObject.NEWLINE);
            ToXml(xml, 0);
            xml.Append(NSObject.NEWLINE);
            xml.Append("</plist>");
            return xml.ToString();
        }

        /// <summary>
        /// Generates the ASCII representation of this object.
        /// The generated ASCII representation does not end with a newline.
        /// Complies with https://developer.apple.com/library/mac/#documentation/Cocoa/Conceptual/PropertyLists/OldStylePlists/OldStylePLists.html
        /// </summary>
        /// <param name="ascii">The StringBuilder onto which the ASCII representation is appended.</param>
        /// <param name="level">The indentation level of the object.</param>
        protected abstract void ToASCII(StringBuilder ascii, int level);

        /// <summary>
        /// Generates the ASCII representation of this object in the GnuStep format.
        /// The generated ASCII representation does not end with a newline.
        /// </summary>
        /// <param name="ascii">The StringBuilder onto which the ASCII representation is appended.</param>
        /// <param name="level">The indentation level of the object.</param>
        protected abstract void ToASCIIGnuStep(StringBuilder ascii, int level);

        /// <summary>
        /// Helper method that adds correct identation to the xml output.
        /// Calling this method will add <code>level</code> number of tab characters
        /// to the <code>xml</code> string.
        /// </summary>
        /// <param name="xml">The string builder for the XML document.</param>
        /// <param name="level">The level of identation.</param>
        internal void Indent(StringBuilder xml, int level) {
            for (int i = 0; i < level; i++)
                xml.Append(INDENT);
        }

        /// <summary>
        /// Wraps the given value inside a NSObject.
        /// </summary>
        /// <param name="value">The value to represent as a NSObject.</param>
        /// <returns>A NSObject representing the given value.</returns>
        public static NSNumber Wrap(long value) {
            return new NSNumber(value);
        }

        /// <summary>
        /// Wraps the given value inside a NSObject.
        /// </summary>
        /// <param name="value">The value to represent as a NSObject.</param>
        /// <returns>A NSObject representing the given value.</returns>
        public static NSNumber Wrap(double value) {
            return new NSNumber(value);
        }

        /// <summary>
        /// Wraps the given value inside a NSObject.
        /// </summary>
        /// <param name="value">The value to represent as a NSObject.</param>
        /// <returns>A NSObject representing the given value.</returns>
        public static NSNumber Wrap(bool value) {
            return new NSNumber(value);
        }

        /// <summary>
        /// Wraps the given value inside a NSObject.
        /// </summary>
        /// <param name="value">The value to represent as a NSObject.</param>
        /// <returns>A NSObject representing the given value.</returns>
        // TODO: Implement NSData class
        /*
        public static NSData Wrap(byte[] value) {
            return new NSData(value);
        }*/

        /// <summary>
        /// Creates a NSArray with the contents of the given array.
        /// </summary>
        /// <param name="value">The value to represent as a NSObject.</param>
        /// <returns>A NSObject representing the given value.</returns>
        /// <exception cref="SystemException">When one of the objects contained in the array cannot be represented by a NSObject.</exception>
        // TODO: Implement NSArray class
        /*
        public static NSArray Wrap(Object[] value) {
            NSArray arr = new NSArray(value.length);
            for (int i = 0; i < value.length; i++) {
                arr.setValue(i, wrap(value[i]));
            }
            return arr;
        }*/

        /// <summary>
        /// Creates a NSDictionary with the contents of the given map.
        /// </summary>
        /// <param name="value">The value to represent as a NSObject.</param>
        /// <returns>A NSObject representing the given value.</returns>
        /// <exception cref="SystemException">When one of the values contained in the map cannot be represented by a NSObject.</exception>
        // TODO: Implement NSDictionary class
        /*
        public static NSDictionary Wrap(Dictionary<String, Object> value) {
            NSDictionary dict = new NSDictionary();
            for (String key : value.keySet())
                dict.put(key, wrap(value.get(key)));
            return dict;
        }*/

        /// <summary>
        /// Creates a NSSet with the contents of this set.
        /// </summary>
        /// <param name="value">The value to represent as a NSObject.</param>
        /// <returns>A NSObject representing the given value.</returns>
        /// <exception cref="SystemException">When one of the values contained in the map cannot be represented by a NSObject.</exception>
        // TODO: Implement NSSet class
        /*
        public static NSSet Wrap(List<Object> value) {
            NSSet set = new NSSet();
            for (Object o : value.toArray())
                set.addObject(wrap(o));
            return set;
        }*/

        /// <summary>
        /// Creates a NSObject representing the given .NET Object.
        ///
        /// Numerics of type <see cref="bool"/>, <see cref="int"/>, <see cref="long"/>, <see cref="short"/>, <see cref="byte"/>, <see cref="float"/> or <see cref="double"/> are wrapped as NSNumber objects.
        ///
        /// Strings are wrapped as NSString objects abd byte arrays as NSData objects.
        ///
        /// Date objects are wrapped as NSDate objects.
        ///
        /// Serializable classes are serialized and their data is stored in NSData objects.
        ///
        /// Arrays and Collection objects are converted to NSArrays where each array member is wrapped into a NSObject.
        ///
        /// Map objects are converted to NSDictionaries. Each key is converted to a string and each value wrapped into a NSObject.
        /// </summary>
        /// <param name="o">The object to represent.</param>
        ///<returns>A NSObject equivalent to the given object.</returns>
        // TODO: Implement all classes
        /*
        public static NSObject Wrap(Object o) {
            if(o == null)
                throw new NullPointerException("A null object cannot be wrapped as a NSObject");

            if(o instanceof NSObject)
                return (NSObject)o;

            Class<?> c = o.getClass();
            if (Boolean.class.equals(c)) {
                return wrap((boolean) (Boolean) o);
            }
            if (Byte.class.equals(c)) {
                return wrap((int) (Byte) o);
            }
            if (Short.class.equals(c)) {
                return wrap((int) (Short) o);
            }
            if (Integer.class.equals(c)) {
                return wrap((int) (Integer) o);
            }
            if (Long.class.isAssignableFrom(c)) {
                return wrap((long) (Long) o);
            }
            if (Float.class.equals(c)) {
                return wrap((double) (Float) o);
            }
            if (Double.class.isAssignableFrom(c)) {
                return wrap((double) (Double) o);
            }
            if (String.class.equals(c)) {
                return new NSString((String)o);
            }
            if (Date.class.equals(c)) {
                return new NSDate((Date)o);
            }
            if(c.isArray()) {
                Class<?> cc = c.getComponentType();
                if (cc.equals(byte.class)) {
                    return wrap((byte[]) o);
                }
                else if(cc.equals(boolean.class)) {
                    boolean[] array = (boolean[])o;
                    NSArray nsa = new NSArray(array.length);
                    for(int i=0;i<array.length;i++)
                        nsa.setValue(i, wrap(array[i]));
                    return nsa;
                }
                else if(float.class.equals(cc)) {
                    float[] array = (float[])o;
                    NSArray nsa = new NSArray(array.length);
                    for(int i=0;i<array.length;i++)
                        nsa.setValue(i, wrap(array[i]));
                    return nsa;
                }
                else if(double.class.equals(cc)) {
                    double[] array = (double[])o;
                    NSArray nsa = new NSArray(array.length);
                    for(int i=0;i<array.length;i++)
                        nsa.setValue(i, wrap(array[i]));
                    return nsa;
                }
                else if(short.class.equals(cc)) {
                    short[] array = (short[])o;
                    NSArray nsa = new NSArray(array.length);
                    for(int i=0;i<array.length;i++)
                        nsa.setValue(i, wrap(array[i]));
                    return nsa;
                }
                else if(int.class.equals(cc)) {
                    int[] array = (int[])o;
                    NSArray nsa = new NSArray(array.length);
                    for(int i=0;i<array.length;i++)
                        nsa.setValue(i, wrap(array[i]));
                    return nsa;
                }
                else if(long.class.equals(cc)) {
                    long[] array = (long[])o;
                    NSArray nsa = new NSArray(array.length);
                    for(int i=0;i<array.length;i++)
                        nsa.setValue(i, wrap(array[i]));
                    return nsa;
                }
                else {
                    return wrap((Object[]) o);
                }
            }
            if (Map.class.isAssignableFrom(c)) {
                Map map = (Map)o;
                Set keys = map.keySet();
                NSDictionary dict = new NSDictionary();
                for(Object key:keys) {
                    Object val = map.get(key);
                    dict.put(String.valueOf(key), wrap(val));
                }
                return dict;
            }
            if (Collection.class.isAssignableFrom(c)) {
                Collection coll = (Collection)o;
                return wrap(coll.toArray());
            }
            return wrapSerialized(o);
        }*/

        /// <summary>
        /// Serializes the given object using Java's default object serialization
        /// and wraps the serialized object in a NSData object.
        /// </summary>
        /// <param name="o">The object to serialize and wrap.</param>
        /// <returns>A NSData object</returns>
        /// <exception cref="SystemException">When the object could not be serialized.</exception>
        // TODO: Implement NSData class
        /*
        public static NSData WrapSerialized(Object o) {
            try {
                ByteArrayOutputStream baos = new ByteArrayOutputStream();
                ObjectOutputStream oos = new ObjectOutputStream(baos);
                oos.writeObject(o);
                return new NSData(baos.toByteArray());
            } catch (IOException ex) {
                throw new SystemException("The given object of class " + o.getClass().toString() + " could not be serialized and stored in a NSData object.");
            }
        }*/

        /// <summary>
        /// Converts this NSObject into an equivalent object
        /// of the Java Runtime Environment.
        /// <ul>
        /// <li>NSArray objects are converted to arrays.</li>
        /// <li>NSDictionary objects are converted to objects extending the java.util.Map class.</li>
        /// <li>NSSet objects are converted to objects extending the java.util.Set class.</li>
        /// <li>NSNumber objects are converted to primitive number values (int, long, double or bool).</li>
        /// <li>NSString objects are converted to String objects.</li>
        /// <li>NSData objects are converted to byte arrays.</li>
        /// <li>NSDate objects are converted to java.util.Date objects.</li>
        /// <li>UID objects are converted to byte arrays.</li>
        /// </ul>
        /// </summary>
        /// <returns>A native java object representing this NSObject's value.</returns>
        // TODO: Implement all classes
        /*
        public Object ToObject() {
            if(this instanceof NSArray) {
                NSObject[] arrayA = ((NSArray)this).getArray();
                Object[] arrayB = new Object[arrayA.length];
                for(int i = 0; i < arrayA.length; i++) {
                    arrayB[i] = arrayA[i].toJavaObject();
                }
                return arrayB;
            } else if (this instanceof NSDictionary) {
                HashMap<String, NSObject> hashMapA = ((NSDictionary)this).getHashMap();
                HashMap<String, Object> hashMapB = new HashMap<String, Object>(hashMapA.size());
                for(String key:hashMapA.keySet()) {
                    hashMapB.put(key, hashMapA.get(key).toJavaObject());
                }
                return hashMapB;
            } else if(this instanceof NSSet) {
                Set<NSObject> setA = ((NSSet)this).getSet();
                Set<Object> setB;
                if(setA instanceof LinkedHashSet) {
                    setB = new LinkedHashSet<Object>(setA.size());
                } else {
                    setB = new TreeSet<Object>();
                }
                for(NSObject o:setA) {
                    setB.add(o.toJavaObject());
                }
                return setB;
            } else if(this instanceof NSNumber) {
                NSNumber num = (NSNumber)this;
                switch(num.type()) {
                    case NSNumber.INTEGER : {
                            long longVal = num.longValue();
                            if(longVal > Integer.MAX_VALUE || longVal < Integer.MIN_VALUE) {
                                return longVal;
                            } else {
                                return num.intValue();
                            }
                        }
                    case NSNumber.REAL : {
                            return num.doubleValue();
                        }
                    case NSNumber.BOOLEAN : {
                            return num.boolValue();
                        }
                    default : {
                            return num.doubleValue();
                        }
                }
            } else if(this instanceof NSString) {
                return ((NSString)this).getContent();
            } else if(this instanceof NSData) {
                return ((NSData)this).bytes();
            } else if(this instanceof NSDate) {
                return ((NSDate)this).getDate();
            } else if(this instanceof UID) {
                return ((UID)this).getBytes();
            } else {
                return this;
            }
        }
        */
    }
}

