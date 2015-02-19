// plist-cil - An open source library to Parse and generate property lists for .NET
// Copyright (C) 2015 Natalia Portillo
//
// This code is based on:
// plist - An open source library to Parse and generate property lists
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
using System.IO;
using NUnit.Framework;
using Claunia.PropertyList;
using System.Collections.Generic;

namespace plistcil.test
{
    [TestFixture]
    public static class ParseTest
    {
        /**
     * Test the xml reader/writer
     */
        [Test]
        public static void TestXml()
        {
            // Parse an example plist file
            NSObject x = PropertyListParser.Parse(new FileInfo("test-files/test1.plist"));

            // check the data in it
            NSDictionary d = (NSDictionary)x;
            Assert.True(d.Count == 5);
            Assert.True(((NSString)d.ObjectForKey("keyA")).ToString().Equals("valueA"));
            Assert.True(((NSString)d.ObjectForKey("key&B")).ToString().Equals("value&B"));
            Assert.True(((NSDate)d.ObjectForKey("date")).Date.Equals(new DateTime(2011, 11, 28, 9, 21, 30, DateTimeKind.Utc)));
            Assert.True(ArrayEquals(((NSData)d.ObjectForKey("data")).Bytes,
                new byte[]{ 0x00, 0x00, 0x00, 0x04, 0x10, 0x41, 0x08, 0x20, (byte)0x82 }));
            NSArray a = (NSArray)d.ObjectForKey("array");
            Assert.True(a.Count == 4);
            Assert.True(a.ObjectAtIndex(0).Equals(new NSNumber(true)));
            Assert.True(a.ObjectAtIndex(1).Equals(new NSNumber(false)));
            Assert.True(a.ObjectAtIndex(2).Equals(new NSNumber(87)));
            Assert.True(a.ObjectAtIndex(3).Equals(new NSNumber(3.14159)));

            // read/write it, make sure we get the same thing
            PropertyListParser.SaveAsXml(x, new FileInfo("test-files/out-testXml.plist"));
            NSObject y = PropertyListParser.Parse(new FileInfo("test-files/out-testXml.plist"));
            Assert.True(x.Equals(y));
        }

        /**
     *  Test the binary reader/writer.
     */
        [Test]
        public static void TestBinary()
        {
            NSObject x = PropertyListParser.Parse(new FileInfo("test-files/test1.plist"));

            // save and load as binary
            PropertyListParser.SaveAsBinary(x, new FileInfo("test-files/out-testBinary.plist"));
            NSObject y = PropertyListParser.Parse(new FileInfo("test-files/out-testBinary.plist"));
            Assert.True(x.Equals(y));
        }

        /**
     *  NSSet only occurs in binary property lists, so we have to test it separately.
     *  NSSets are not yet supported in reading/writing, as binary property list format v1+ is required.
     */
        /*
        [Test]
        public static void TestSet()
        {
            NSSet s = new NSSet();
            s.AddObject(new NSNumber(1));
            s.AddObject(new NSNumber(3));
            s.AddObject(new NSNumber(2));

            NSSet orderedSet = new NSSet(true);
            s.AddObject(new NSNumber(1));
            s.AddObject(new NSNumber(3));
            s.AddObject(new NSNumber(2));

            NSDictionary dict = new NSDictionary();
            dict.Add("set1", s);
            dict.Add("set2", orderedSet);

            PropertyListParser.SaveAsBinary(dict, new FileInfo("test-files/out-testSet.plist"));
            NSObject ParsedRoot = PropertyListParser.Parse(new FileInfo("test-files/out-testSet.plist"));
            Assert.True(ParsedRoot.Equals(dict));
        }*/

        [Test]
        public static void TestASCII()
        {
            NSObject x = PropertyListParser.Parse(new FileInfo("test-files/test1-ascii.plist"));
            NSDictionary d = (NSDictionary)x;
            Assert.True(d.Count == 5);
            Assert.True(((NSString)d.ObjectForKey("keyA")).ToString().Equals("valueA"));
            Assert.True(((NSString)d.ObjectForKey("key&B")).ToString().Equals("value&B"));
            Assert.True(((NSDate)d.ObjectForKey("date")).Date.Equals(new DateTime(2011, 11, 28, 9, 21, 30, DateTimeKind.Utc)));
            Assert.True(ArrayEquals(((NSData)d.ObjectForKey("data")).Bytes,
                new byte[]{ 0x00, 0x00, 0x00, 0x04, 0x10, 0x41, 0x08, 0x20, (byte)0x82 }));
            NSArray a = (NSArray)d.ObjectForKey("array");
            Assert.True(a.Count == 4);
            Assert.True(a.ObjectAtIndex(0).Equals(new NSString("YES")));
            Assert.True(a.ObjectAtIndex(1).Equals(new NSString("NO")));
            Assert.True(a.ObjectAtIndex(2).Equals(new NSString("87")));
            Assert.True(a.ObjectAtIndex(3).Equals(new NSString("3.14159")));
        }

        [Test]
        public static void TestGnuStepASCII()
        {
            NSObject x = PropertyListParser.Parse(new FileInfo("test-files/test1-ascii-gnustep.plist"));
            NSDictionary d = (NSDictionary)x;
            Assert.True(d.Count == 5);
            Assert.True(((NSString)d.ObjectForKey("keyA")).ToString().Equals("valueA"));
            Assert.True(((NSString)d.ObjectForKey("key&B")).ToString().Equals("value&B"));
            Assert.True(((NSDate)d.ObjectForKey("date")).Date.Equals(new DateTime(2011, 11, 28, 9, 21, 30, DateTimeKind.Utc)));
            Assert.True(ArrayEquals(((NSData)d.ObjectForKey("data")).Bytes,
                new byte[]{ 0x00, 0x00, 0x00, 0x04, 0x10, 0x41, 0x08, 0x20, (byte)0x82 }));
            NSArray a = (NSArray)d.ObjectForKey("array");
            Assert.True(a.Count == 4);
            Assert.True(a.ObjectAtIndex(0).Equals(new NSNumber(true)));
            Assert.True(a.ObjectAtIndex(1).Equals(new NSNumber(false)));
            Assert.True(a.ObjectAtIndex(2).Equals(new NSNumber(87)));
            Assert.True(a.ObjectAtIndex(3).Equals(new NSNumber(3.14159)));
        }

        [Test]
        public static void TestASCIIWriting()
        {
            FileInfo inf = new FileInfo("test-files/test1.plist");
            FileInfo outf = new FileInfo("test-files/out-test1-ascii.plist");
            FileInfo in2 = new FileInfo("test-files/test1-ascii.plist");
            NSDictionary x = (NSDictionary)PropertyListParser.Parse(inf);
            PropertyListParser.SaveAsASCII(x, outf);

            //Information gets lost when saving into the ASCII format (NSNumbers are converted to NSStrings)

            NSDictionary y = (NSDictionary)PropertyListParser.Parse(outf);
            NSDictionary z = (NSDictionary)PropertyListParser.Parse(in2);
            Assert.True(y.Equals(z));
        }

        [Test]
        public static void TestGnuStepASCIIWriting()
        {
            FileInfo inf = new FileInfo("test-files/test1.plist");
            FileInfo outf = new FileInfo("test-files/out-test1-ascii-gnustep.plist");
            NSDictionary x = (NSDictionary)PropertyListParser.Parse(inf);
            PropertyListParser.SaveAsGnuStepASCII(x, outf);
            NSObject y = PropertyListParser.Parse(outf);
            Assert.True(x.Equals(y));
        }

        [Test]
        public static void TestWrap()
        {
            bool bl = true;
            byte byt = 24;
            short shrt = 12;
            int i = 42;
            long lng = 30000000000L;
            float flt = 124.3f;
            double dbl = 32.0;
            DateTime date = new DateTime();
            string strg = "Hello World";
            byte[] bytes = new byte[] { (byte)0x00, (byte)0xAF, (byte)0xAF };
            ArgIterator netObject = new ArgIterator();
            Object[] array = new Object[] { bl, byt, shrt, i, lng, flt, dbl, date, strg, bytes };
            int[] array2 = new int[] { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 3000 };
            List<Object> list = new List<Object>(array);

            Dictionary<string, Object> map = new Dictionary<string, Object>();
            map.Add("int", i);
            map.Add("long", lng);
            map.Add("date", date);

            NSObject WrappedO = NSObject.Wrap((Object)bl);
            Assert.True(WrappedO.GetType().Equals(typeof(NSNumber)));
            Assert.True(WrappedO.ToObject().Equals(bl));

            WrappedO = NSObject.Wrap((Object)byt);
            Assert.True(WrappedO.GetType().Equals(typeof(NSNumber)));
            Assert.True((int)WrappedO.ToObject() == byt);

            WrappedO = NSObject.Wrap((Object)shrt);
            Assert.True(WrappedO.GetType().Equals(typeof(NSNumber)));
            Assert.True((int)WrappedO.ToObject() == shrt);

            WrappedO = NSObject.Wrap((Object)i);
            Assert.True(WrappedO.GetType().Equals(typeof(NSNumber)));
            Assert.True((int)WrappedO.ToObject() == i);

            WrappedO = NSObject.Wrap((Object)lng);
            Assert.True(WrappedO.GetType().Equals(typeof(NSNumber)));
            Assert.True((long)WrappedO.ToObject() == lng);

            WrappedO = NSObject.Wrap((Object)flt);
            Assert.True(WrappedO.GetType().Equals(typeof(NSNumber)));
            Assert.True((double)WrappedO.ToObject() == flt);

            WrappedO = NSObject.Wrap((Object)dbl);
            Assert.True(WrappedO.GetType().Equals(typeof(NSNumber)));
            Assert.True((double)WrappedO.ToObject() == dbl);

            WrappedO = NSObject.Wrap((Object)date);
            Assert.True(WrappedO.GetType().Equals(typeof(NSDate)));
            Assert.True(((DateTime)WrappedO.ToObject()).Equals(date));

            WrappedO = NSObject.Wrap((Object)strg);
            Assert.True(WrappedO.GetType().Equals(typeof(NSString)));
            Assert.True(((string)WrappedO.ToObject()).Equals(strg));

            WrappedO = NSObject.Wrap((Object)bytes);
            Assert.True(WrappedO.GetType().Equals(typeof(NSData)));
            byte[] data = (byte[])WrappedO.ToObject();
            Assert.True(data.Length == bytes.Length);
            for (int x = 0; x < bytes.Length; x++)
                Assert.True(data[x] == bytes[x]);

            WrappedO = NSObject.Wrap((Object)array);
            Assert.True(WrappedO.GetType().Equals(typeof(NSArray)));
            Object[] objArray = (Object[])WrappedO.ToObject();
            Assert.True(objArray.Length == array.Length);

            WrappedO = NSObject.Wrap((Object)array2);
            Assert.True(WrappedO.GetType().Equals(typeof(NSArray)));
            Assert.True(((NSArray)WrappedO).Count == array2.Length);

            WrappedO = NSObject.Wrap((Object)list);
            Assert.True(WrappedO.GetType().Equals(typeof(NSArray)));
            objArray = (Object[])WrappedO.ToObject();
            Assert.True(objArray.Length == array.Length);

            Assert.True(NSObject.Wrap((Object)netObject).GetType().Equals(typeof(NSData)));

            WrappedO = NSObject.Wrap((Object)map);
            Assert.True(WrappedO.GetType().Equals(typeof(NSDictionary)));
            NSDictionary dict = (NSDictionary)WrappedO;
            Assert.True(((NSNumber)dict.ObjectForKey("int")).ToLong() == i);
            Assert.True(((NSNumber)dict.ObjectForKey("long")).ToLong() == lng);
            Assert.True(((NSDate)dict.ObjectForKey("date")).Date.Equals(date));

            // TODO
            /*
        Object unWrappedO = WrappedO.ToObject();
        Map map2 = (Map)unWrappedO;
        Assert.True(((int)map.get("int")) == i);
        Assert.True(((long)map.get("long")) == lng);
        Assert.True(((DateTime)map.get("date")).Equals(date));*/
        }

        static bool ArrayEquals(byte[] arrayA, byte[] arrayB)
        {
            if (arrayA.Length == arrayB.Length)
            {
                for (int i = 0; i < arrayA.Length; i++)
                {
                    if (arrayA[i] != arrayB[i])
                    {
                        return false;
                    }
                }
                return true;
            }
            return false;
        }

    }
}

