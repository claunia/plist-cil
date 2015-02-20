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
using System.IO;

namespace Claunia.PropertyList
{
    /// <summary>
    /// Parses property lists that are in Apple's binary format.
    /// Use this class when you are sure about the format of the property list.
    /// Otherwise use the PropertyListParser class.
    ///
    /// Parsing is done by calling the static <see cref="Parse"/> methods.
    ///
    /// </summary>
    /// @author Daniel Dreibrodt
    /// @author Natalia Portillo
    public class BinaryPropertyListParser
    {
        int majorVersion, minorVersion;

        /**
     * property list in bytes *
     */
        byte[] bytes;
        /**
     * Length of an offset definition in bytes *
     */
        int offsetSize;
        /**
     * Length of an object reference in bytes *
     */
        int objectRefSize;
        /**
     * Number of objects stored in this property list *
     */
        int numObjects;
        /**
     * Reference to the top object of the property list *
     */
        int topObject;
        /**
     * Offset of the offset table from the beginning of the file *
     */
        int offsetTableOffset;
        /**
     * The table holding the information at which offset each object is found *
     */
        int[] offsetTable;

        /// <summary>
        /// Protected constructor so that instantiation is fully controlled by the
        /// static parse methods.
        /// </summary>
        /// <see cref="Parse(byte[])"/>
        protected BinaryPropertyListParser()
        {
            /** empty **/
        }

        /// <summary>
        /// Parses a binary property list from a byte array.
        /// </summary>
        /// <param name="data">The binary property list's data.</param>
        /// <returns>The root object of the property list. This is usually a NSDictionary but can also be a NSArray.</returns>
        /// <exception cref="PropertyListFormatException">When the property list's format could not be parsed.</exception>
        public static NSObject Parse(byte[] data)
        {
            BinaryPropertyListParser parser = new BinaryPropertyListParser();
            return parser.DoParse(data);
        }

        /// <summary>
        /// Parses a binary property list from a byte array.
        /// </summary>
        /// <returns>The root object of the property list. This is usually a NSDictionary but can also be a NSArray.</returns>
        /// <param name="data">The binary property list's data.</param>
        /// <exception cref="PropertyListFormatException">When the property list's format could not be parsed.</exception>
        NSObject DoParse(byte[] data)
        {
            bytes = data;
            string magic = Encoding.ASCII.GetString(CopyOfRange(bytes, 0, 8));
            if (!magic.StartsWith("bplist", StringComparison.Ordinal))
            {
                throw new PropertyListFormatException("The given data is no binary property list. Wrong magic bytes: " + magic);
            }

            majorVersion = magic[6] - 0x30; //ASCII number
            minorVersion = magic[7] - 0x30; //ASCII number

            // 0.0 - OS X Tiger and earlier
            // 0.1 - Leopard
            // 0.? - Snow Leopard
            // 1.5 - Lion
            // 2.0 - Snow Lion

            if (majorVersion > 0)
            {
                throw new PropertyListFormatException("Unsupported binary property list format: v" + majorVersion + "." + minorVersion + ". " +
                "Version 1.0 and later are not yet supported.");
            }

            /*
         * Handle trailer, last 32 bytes of the file
         */
            byte[] trailer = CopyOfRange(bytes, bytes.Length - 32, bytes.Length);
            //6 null bytes (index 0 to 5)
            offsetSize = (int)ParseUnsignedInt(trailer, 6, 7);
            //System.Console.WriteLine("offsetSize: "+offsetSize);
            objectRefSize = (int)ParseUnsignedInt(trailer, 7, 8);
            //System.Console.WriteLine("objectRefSize: "+objectRefSize);
            numObjects = (int)ParseUnsignedInt(trailer, 8, 16);
            //System.Console.WriteLine("numObjects: "+numObjects);
            topObject = (int)ParseUnsignedInt(trailer, 16, 24);
            //System.Console.WriteLine("topObject: "+topObject);
            offsetTableOffset = (int)ParseUnsignedInt(trailer, 24, 32);
            //System.Console.WriteLine("offsetTableOffset: "+offsetTableOffset);

            /*
         * Handle offset table
         */
            offsetTable = new int[numObjects];

            for (int i = 0; i < numObjects; i++)
            {
                byte[] offsetBytes = CopyOfRange(bytes, offsetTableOffset + i * offsetSize, offsetTableOffset + (i + 1) * offsetSize);
                offsetTable[i] = (int)ParseUnsignedInt(offsetBytes);
                /*System.Console.Write("Offset for Object #"+i+" is "+offsetTable[i]+" [");
            foreach(byte b: in ffsetBytes) System.Console.Write(Convert.ToString((int)b, 16))+" ");
            System.Console.WriteLine("]");*/
            }

            return ParseObject(topObject);
        }

        /// <summary>
        /// Parses a binary property list from an input stream.
        /// </summary>
        /// <param name="fs">The input stream that points to the property list's data.</param>
        /// <returns>The root object of the property list. This is usually a NSDictionary but can also be a NSArray.</returns>
        /// <exception cref="PropertyListFormatException">When the property list's format could not be parsed.</exception>
        public static NSObject Parse(Stream fs)
        {
            //Read all bytes into a list
            byte[] buf = PropertyListParser.ReadAll(fs);
            fs.Close();
            return Parse(buf);
        }

        /// <summary>
        /// Parses a binary property list file.
        /// </summary>
        /// <param name="f">The binary property list file</param>
        /// <returns>The root object of the property list. This is usually a NSDictionary but can also be a NSArray.</returns>
        /// <exception cref="PropertyListFormatException">When the property list's format could not be parsed.</exception>
        public static NSObject Parse(FileInfo f)
        {
            // While on Java, heap size is limited by the JVM, on .NET the heap size is dynamically allocated using all
            // available RAM+swap. There is a function to check if that allocation can succeed, but works in 16MiB pieces,
            // far bigger than any known PropertyList. And even then, paging would allow to work with insanely sized PropertyLists.
            // Therefor, the checks in .NET (System.Runtime.MemoryFailPoint) are not worth the effort.
            // Rest of calls to Java's Runtime.getRuntime().freeMemory() will not be commented but completely removed.
            /*
            if (f.length() > Runtime.getRuntime().freeMemory()) {
                throw new OutOfMemoryError("To little heap space available! Wanted to read " + f.length() + " bytes, but only " + Runtime.getRuntime().freeMemory() + " are available.");
            }*/
            return Parse(f.OpenRead());
        }

        /// <summary>
        /// Parses an object inside the currently parsed binary property list.
        /// For the format specification check
        /// <a href="http://www.opensource.apple.com/source/CF/CF-744/CFBinaryPList.c">
        /// Apple's binary property list parser implementation</a>.
        /// </summary>
        /// <returns>The parsed object.</returns>
        /// <param name="obj">The object ID.</param>
        /// <exception cref="PropertyListFormatException">When the property list's format could not be parsed.</exception>
        NSObject ParseObject(int obj)
        {
            int offset = offsetTable[obj];
            byte type = bytes[offset];
            int objType = (type & 0xF0) >> 4; //First  4 bits
            int objInfo = (type & 0x0F);      //Second 4 bits
            switch (objType)
            {
                case 0x0:
                    {
                        //Simple
                        switch (objInfo)
                        {
                            case 0x0:
                                {
                                    //null object (v1.0 and later)
                                    return null;
                                }
                            case 0x8:
                                {
                                    //false
                                    return new NSNumber(false);
                                }
                            case 0x9:
                                {
                                    //true
                                    return new NSNumber(true);
                                }
                            case 0xC:
                                {
                                    //URL with no base URL (v1.0 and later)
                                    //TODO
                                    break;
                                }
                            case 0xD:
                                {
                                    //URL with base URL (v1.0 and later)
                                    //TODO
                                    break;
                                }
                            case 0xE:
                                {
                                    //16-byte UUID (v1.0 and later)
                                    //TODO
                                    break;
                                }
                            case 0xF:
                                {
                                    //filler byte
                                    return null;
                                }
                        }
                        break;
                    }
                case 0x1:
                    {
                        //integer
                        int length = (int)Math.Pow(2, objInfo);
                        return new NSNumber(CopyOfRange(bytes, offset + 1, offset + 1 + length), NSNumber.INTEGER);
                    }
                case 0x2:
                    {
                        //real
                        int length = (int)Math.Pow(2, objInfo);
                        return new NSNumber(CopyOfRange(bytes, offset + 1, offset + 1 + length), NSNumber.REAL);
                    }
                case 0x3:
                    {
                        //Date
                        if (objInfo != 0x3)
                        {
                            throw new PropertyListFormatException("The given binary property list contains a date object of an unknown type (" + objInfo + ")");
                        }
                        return new NSDate(CopyOfRange(bytes, offset + 1, offset + 9));
                    }
                case 0x4:
                    {
                        //Data
                        int[] lenAndoffset = ReadLengthAndOffset(objInfo, offset);
                        int length = lenAndoffset[0];
                        int dataoffset = lenAndoffset[1];

                        return new NSData(CopyOfRange(bytes, offset + dataoffset, offset + dataoffset + length));
                    }
                case 0x5:
                    {
                        //ASCII String
                        int[] lenAndoffset = ReadLengthAndOffset(objInfo, offset);
                        int length = lenAndoffset[0];
                        int stroffset = lenAndoffset[1];

                        return new NSString(CopyOfRange(bytes, offset + stroffset, offset + stroffset + length), "ASCII");
                    }
                case 0x6:
                    {
                        //UTF-16-BE String
                        int[] lenAndoffset = ReadLengthAndOffset(objInfo, offset);
                        int length = lenAndoffset[0];
                        int stroffset = lenAndoffset[1];

                        //length is String length -> to get byte length multiply by 2, as 1 character takes 2 bytes in UTF-16
                        length *= 2;
                        return new NSString(CopyOfRange(bytes, offset + stroffset, offset + stroffset + length), "UTF-16BE");
                    }
                case 0x8:
                    {
                        //UID
                        int length = objInfo + 1;
                        return new UID(obj.ToString(), CopyOfRange(bytes, offset + 1, offset + 1 + length));
                    }
                case 0xA:
                    {
                        //Array
                        int[] lenAndoffset = ReadLengthAndOffset(objInfo, offset);
                        int length = lenAndoffset[0];
                        int arrayoffset = lenAndoffset[1];

                        NSArray array = new NSArray(length);
                        for (int i = 0; i < length; i++)
                        {
                            int objRef = (int)ParseUnsignedInt(CopyOfRange(bytes,
                                             offset + arrayoffset + i * objectRefSize,
                                             offset + arrayoffset + (i + 1) * objectRefSize));
                            array.SetValue(i, ParseObject(objRef));
                        }
                        return array;

                    }
                case 0xB:
                    {
                        //Ordered set
                        int[] lenAndoffset = ReadLengthAndOffset(objInfo, offset);
                        int length = lenAndoffset[0];
                        int contentOffset = lenAndoffset[1];

                        NSSet set = new NSSet(true);
                        for (int i = 0; i < length; i++)
                        {
                            int objRef = (int)ParseUnsignedInt(CopyOfRange(bytes,
                                             offset + contentOffset + i * objectRefSize,
                                             offset + contentOffset + (i + 1) * objectRefSize));
                            set.AddObject(ParseObject(objRef));
                        }
                        return set;
                    }
                case 0xC:
                    {
                        //Set
                        int[] lenAndoffset = ReadLengthAndOffset(objInfo, offset);
                        int length = lenAndoffset[0];
                        int contentOffset = lenAndoffset[1];

                        NSSet set = new NSSet();
                        for (int i = 0; i < length; i++)
                        {
                            int objRef = (int)ParseUnsignedInt(CopyOfRange(bytes,
                                             offset + contentOffset + i * objectRefSize,
                                             offset + contentOffset + (i + 1) * objectRefSize));
                            set.AddObject(ParseObject(objRef));
                        }
                        return set;
                    }
                case 0xD:
                    {
                        //Dictionary
                        int[] lenAndoffset = ReadLengthAndOffset(objInfo, offset);
                        int length = lenAndoffset[0];
                        int contentOffset = lenAndoffset[1];

                        //System.out.println("Parsing dictionary #"+obj);
                        NSDictionary dict = new NSDictionary();
                        for (int i = 0; i < length; i++)
                        {
                            int keyRef = (int)ParseUnsignedInt(CopyOfRange(bytes,
                                             offset + contentOffset + i * objectRefSize,
                                             offset + contentOffset + (i + 1) * objectRefSize));
                            int valRef = (int)ParseUnsignedInt(CopyOfRange(bytes,
                                             offset + contentOffset + (length * objectRefSize) + i * objectRefSize,
                                             offset + contentOffset + (length * objectRefSize) + (i + 1) * objectRefSize));
                            NSObject key = ParseObject(keyRef);
                            NSObject val = ParseObject(valRef);
                            dict.Add(key.ToString(), val);
                        }
                        return dict;
                    }
                default:
                    {
                        Console.WriteLine("WARNING: The given binary property list contains an object of unknown type (" + objType + ")");
                        break;
                    }
            }
            return null;
        }

        /// <summary>
        /// Reads the length for arrays, sets and dictionaries.
        /// </summary>
        /// <returns>An array with the length two. First entry is the length, second entry the offset at which the content starts.</returns>
        /// <param name="objInfo">Object information byte.</param>
        /// <param name="offset">Offset in the byte array at which the object is located.</param>
        int[] ReadLengthAndOffset(int objInfo, int offset)
        {
            int length = objInfo;
            int stroffset = 1;
            if (objInfo == 0xF)
            {
                int int_type = bytes[offset + 1];
                int intType = (int_type & 0xF0) >> 4;
                if (intType != 0x1)
                {
                    Console.WriteLine("BinaryPropertyListParser: Length integer has an unexpected type" + intType + ". Attempting to parse anyway...");
                }
                int intInfo = int_type & 0x0F;
                int intLength = (int)Math.Pow(2, intInfo);
                stroffset = 2 + intLength;
                if (intLength < 3)
                {
                    length = (int)ParseUnsignedInt(CopyOfRange(bytes, offset + 2, offset + 2 + intLength));
                }
                else
                {
                    // BigInteger is Little-Endian in .NET, swap the thing.
                    // Also BigInteger is of .NET 4.0, maybe there's a better way to do it.
                    byte[] bigEBigInteger = CopyOfRange(bytes, offset + 2, offset + 2 + intLength);
                    byte[] litEBigInteger = new byte[bigEBigInteger.Length + 1];
                    for (int i = 0, j = bigEBigInteger.Length - 1; i < bigEBigInteger.Length && j >= 0; i++, j--)
                        litEBigInteger[i] = bigEBigInteger[j];
                    litEBigInteger[litEBigInteger.Length - 1] = (byte)0x00; // Be sure to get unsigned BigInteger

                    length = (int)new System.Numerics.BigInteger(litEBigInteger);
                }
            }
            return new []{ length, stroffset };
        }

        /// <summary>
        /// Parses an unsigned integers from a byte array.
        /// </summary>
        /// <returns>The byte array containing the unsigned integer.</returns>
        /// <param name="bytes">The unsigned integer represented by the given bytes.</param>
        public static long ParseUnsignedInt(byte[] bytes)
        {
            long l = 0;
            foreach (byte b in bytes)
            {
                l <<= 8;
                #pragma warning disable 675
                l |= b & 0xFF;
                #pragma warning restore 675
            }
            l &= 0xFFFFFFFFL;
            return l;
        }

        /// <summary>
        /// Parses an unsigned integer from a byte array.
        /// </summary>
        /// <returns>The unsigned integer represented by the given bytes.</returns>
        /// <param name="bytes">The byte array containing the unsigned integer.</param>
        /// <param name="startIndex">Beginning of the unsigned int in the byte array.</param>
        /// <param name="endIndex">End of the unsigned int in the byte array.</param>
        public static long ParseUnsignedInt(byte[] bytes, int startIndex, int endIndex)
        {
            long l = 0;
            for (int i = startIndex; i < endIndex; i++)
            {
                l <<= 8;
                #pragma warning disable 675
                l |= bytes[i] & 0xFF;
                #pragma warning restore 675
            }
            l &= 0xFFFFFFFFL;
            return l;
        }

        /// <summary>
        /// Parses a long from a (big-endian) byte array.
        /// </summary>
        /// <returns>The long integer represented by the given bytes.</returns>
        /// <param name="bytes">The bytes representing the long integer.</param>
        public static long ParseLong(byte[] bytes)
        {
            long l = 0;
            foreach (byte b in bytes)
            {
                l <<= 8;
                #pragma warning disable 675
                l |= b & 0xFF;
                #pragma warning restore 675
            }
            return l;
        }

        /// <summary>
        /// Parses a long from a (big-endian) byte array.
        /// </summary>
        /// <returns>The long integer represented by the given bytes.</returns>
        /// <param name="bytes">The bytes representing the long integer.</param>
        /// <param name="startIndex">Beginning of the long in the byte array.</param>
        /// <param name="endIndex">End of the long in the byte array.</param>
        public static long ParseLong(byte[] bytes, int startIndex, int endIndex)
        {
            long l = 0;
            for (int i = startIndex; i < endIndex; i++)
            {
                l <<= 8;
                #pragma warning disable 675
                l |= bytes[i] & 0xFF;
                #pragma warning restore 675
            }
            return l;
        }

        /// <summary>
        /// Parses a double from a (big-endian) byte array.
        /// </summary>
        /// <returns>The double represented by the given bytes.</returns>
        /// <param name="bytes">The bytes representing the double.</param>
        public static double ParseDouble(byte[] bytes)
        {
            if (bytes.Length == 8)
                return BitConverter.Int64BitsToDouble(ParseLong(bytes));
            if (bytes.Length == 4)
                return BitConverter.ToSingle(BitConverter.GetBytes(ParseLong(bytes)), 0);
            throw new ArgumentException("bad byte array length " + bytes.Length);
        }

        /// <summary>
        /// Parses a double from a (big-endian) byte array.
        /// </summary>
        /// <returns>The double represented by the given bytes.</returns>
        /// <param name="bytes">The bytes representing the double.</param>
        /// <param name="startIndex">Beginning of the double in the byte array.</param>
        /// <param name="endIndex">End of the double in the byte array.</param>
        public static double ParseDouble(byte[] bytes, int startIndex, int endIndex)
        {
            if (endIndex - startIndex == 8)
                return BitConverter.Int64BitsToDouble(ParseLong(bytes, startIndex, endIndex));
            if (endIndex - startIndex == 4)
                return BitConverter.ToSingle(BitConverter.GetBytes(ParseLong(bytes, startIndex, endIndex)), 0);
            throw new ArgumentException("endIndex (" + endIndex + ") - startIndex (" + startIndex + ") != 4 or 8");
        }

        /// <summary>
        /// Copies a part of a byte array into a new array.
        /// </summary>
        /// <returns>The copied array.</returns>
        /// <param name="src">The source array.</param>
        /// <param name="startIndex">The index from which to start copying.</param>
        /// <param name="endIndex">The index until which to copy.</param>
        public static byte[] CopyOfRange(byte[] src, int startIndex, int endIndex)
        {
            int length = endIndex - startIndex;
            if (length < 0)
            {
                throw new ArgumentOutOfRangeException("startIndex (" + startIndex + ")" + " > endIndex (" + endIndex + ")");
            }
            byte[] dest = new byte[length];
            Array.Copy(src, startIndex, dest, 0, length);
            return dest;
        }
    }
}

