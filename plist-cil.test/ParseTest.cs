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
using System.Collections.Generic;
using System.IO;
using Claunia.PropertyList;
using Xunit;

namespace plistcil.test;

public static class ParseTest
{
    static bool ArrayEquals(byte[] arrayA, byte[] arrayB)
    {
        if(arrayA.Length != arrayB.Length) return false;

        for(int i = 0; i < arrayA.Length; i++)
            if(arrayA[i] != arrayB[i]) return false;

        return true;
    }

    /**
     * NSSet only occurs in binary property lists, so we have to test it separately.
     * NSSets are not yet supported in reading/writing, as binary property list format v1+ is required.
     */
    /*
    [Fact]
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
    [Fact]
    public static void TestASCII()
    {
        NSObject x = PropertyListParser.Parse(new FileInfo("test-files/test1-ascii.plist"));
        var      d = (NSDictionary)x;
        Assert.True(d.Count == 5);
        Assert.Equal("valueA",  ((NSString)d.ObjectForKey("keyA")).ToString());
        Assert.Equal("value&B", ((NSString)d.ObjectForKey("key&B")).ToString());

        var      actualDate   = (NSDate)d.ObjectForKey("date");
        DateTime expectedDate = new DateTime(2011, 11, 28, 9, 21, 30, DateTimeKind.Utc).ToLocalTime();

        Assert.Equal(actualDate.Date, expectedDate);

        Assert.True(ArrayEquals(((NSData)d.ObjectForKey("data")).Bytes,
                                [
                                    0x00, 0x00, 0x00, 0x04, 0x10, 0x41, 0x08, 0x20, 0x82
                                ]));

        var a = (NSArray)d.ObjectForKey("array");
        Assert.True(a.Count == 4);
        Assert.True(a[0].Equals(new NSString("YES")));
        Assert.True(a[1].Equals(new NSString("NO")));
        Assert.True(a[2].Equals(new NSString("87")));
        Assert.True(a[3].Equals(new NSString("3.14159")));
    }

    [Fact]
    public static void testAsciiUtf8CharactersInQuotedString()
    {
        NSObject x = PropertyListParser.Parse(new FileInfo("test-files/test-ascii-utf8.plist"));
        var      d = (NSDictionary)x;
        Assert.Equal(2,                  d.Count);
        Assert.Equal("JÔÖú@2x.jpg",   d.ObjectForKey("path").ToString());
        Assert.Equal("QÔÖú@2x 啕.jpg", d.ObjectForKey("Key QÔÖª@2x 䌡").ToString());
    }

    [Fact]
    public static void TestASCIIWriting()
    {
        var inf  = new FileInfo("test-files/test1.plist");
        var outf = new FileInfo("test-files/out-test1-ascii.plist");
        var in2  = new FileInfo("test-files/test1-ascii.plist");
        var x    = (NSDictionary)PropertyListParser.Parse(inf);
        PropertyListParser.SaveAsASCII(x, outf);

        //Information gets lost when saving into the ASCII format (NSNumbers are converted to NSStrings)

        var y = (NSDictionary)PropertyListParser.Parse(outf);
        var z = (NSDictionary)PropertyListParser.Parse(in2);
        Assert.True(y.Equals(z));
    }

    /**
     * Test the binary reader/writer.
     */
    [Fact]
    public static void TestBinary()
    {
        NSObject x = PropertyListParser.Parse(new FileInfo("test-files/test1.plist"));

        // save and load as binary
        PropertyListParser.SaveAsBinary(x, new FileInfo("test-files/out-testBinary.plist"));
        NSObject y = PropertyListParser.Parse(new FileInfo("test-files/out-testBinary.plist"));
        Assert.True(x.Equals(y));
    }

    [Fact]
    public static void TestGnuStepASCII()
    {
        NSObject x = PropertyListParser.Parse(new FileInfo("test-files/test1-ascii-gnustep.plist"));
        var      d = (NSDictionary)x;
        Assert.True(d.Count == 5);
        Assert.Equal("valueA",  ((NSString)d.ObjectForKey("keyA")).ToString());
        Assert.Equal("value&B", ((NSString)d.ObjectForKey("key&B")).ToString());

        Assert.True(((NSDate)d.ObjectForKey("date")).Date.Equals(new DateTime(2011, 11, 28, 9, 21, 30, DateTimeKind.Utc)
                                                                    .ToLocalTime()));

        Assert.True(ArrayEquals(((NSData)d.ObjectForKey("data")).Bytes,
                                [
                                    0x00, 0x00, 0x00, 0x04, 0x10, 0x41, 0x08, 0x20, 0x82
                                ]));

        var a = (NSArray)d.ObjectForKey("array");
        Assert.True(a.Count == 4);
        Assert.True(a[0].Equals(new NSNumber(true)));
        Assert.True(a[1].Equals(new NSNumber(false)));
        Assert.True(a[2].Equals(new NSNumber(87)));
        Assert.True(a[3].Equals(new NSNumber(3.14159)));
    }

    [Fact]
    public static void TestGnuStepASCIIWriting()
    {
        var inf  = new FileInfo("test-files/test1.plist");
        var outf = new FileInfo("test-files/out-test1-ascii-gnustep.plist");
        var x    = (NSDictionary)PropertyListParser.Parse(inf);
        PropertyListParser.SaveAsGnuStepASCII(x, outf);
        NSObject y = PropertyListParser.Parse(outf);
        Assert.True(x.Equals(y));
    }

    [Fact]
    public static void TestWrap()
    {
        bool   bl   = true;
        byte   byt  = 24;
        short  shrt = 12;
        int    i    = 42;
        long   lng  = 30000000000L;
        float  flt  = 124.3f;
        double dbl  = 32.0;
        var    date = new DateTime();
        string strg = "Hello World";

        byte[] bytes =
        [
            0x00, 0xAF, 0xAF
        ];

        object[] array =
        [
            bl, byt, shrt, i, lng, flt, dbl, date, strg, bytes
        ];

        int[] array2 =
        [
            1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 3000
        ];

        List<object> list = new(array);

        Dictionary<string, object> map = new();
        map.Add("int",  i);
        map.Add("long", lng);
        map.Add("date", date);

        List<Dictionary<string, object>> listOfMaps =
        [
            new Dictionary<string, object>
            {
                {
                    "int", i
                },
                {
                    "long", lng
                },
                {
                    "date", date
                }
            }
        ];

        var WrappedO = NSObject.Wrap((object)bl);
        Assert.True(WrappedO is (NSNumber));
        Assert.True(WrappedO.ToObject().Equals(bl));

        WrappedO = NSObject.Wrap((object)byt);
        Assert.True(WrappedO is (NSNumber));
        Assert.True((int)WrappedO.ToObject() == byt);

        WrappedO = NSObject.Wrap((object)shrt);
        Assert.True(WrappedO is (NSNumber));
        Assert.True((int)WrappedO.ToObject() == shrt);

        WrappedO = NSObject.Wrap((object)i);
        Assert.True(WrappedO is (NSNumber));
        Assert.True((int)WrappedO.ToObject() == i);

        WrappedO = NSObject.Wrap((object)lng);
        Assert.True(WrappedO is (NSNumber));
        Assert.True((long)WrappedO.ToObject() == lng);

        WrappedO = NSObject.Wrap((object)flt);
        Assert.True(WrappedO is (NSNumber));
        Assert.True((double)WrappedO.ToObject() == flt);

        WrappedO = NSObject.Wrap((object)dbl);
        Assert.True(WrappedO is (NSNumber));
        Assert.True((double)WrappedO.ToObject() == dbl);

        WrappedO = NSObject.Wrap(date);
        Assert.True(WrappedO is (NSDate));
        Assert.True(((DateTime)WrappedO.ToObject()).Equals(date));

        WrappedO = NSObject.Wrap(strg);
        Assert.True(WrappedO is (NSString));
        Assert.Equal((string)WrappedO.ToObject(), strg);

        WrappedO = NSObject.Wrap((object)bytes);
        Assert.True(WrappedO is (NSData));
        byte[] data = (byte[])WrappedO.ToObject();
        Assert.True(data.Length == bytes.Length);

        for(int x = 0; x < bytes.Length; x++) Assert.True(data[x] == bytes[x]);

        WrappedO = NSObject.Wrap((object)array);
        Assert.True(WrappedO is (NSArray));
        object[] objArray = (object[])WrappedO.ToObject();
        Assert.True(objArray.Length == array.Length);

        WrappedO = NSObject.Wrap(array2);
        Assert.True(WrappedO is (NSArray));
        Assert.True(((NSArray)WrappedO).Count == array2.Length);

        WrappedO = NSObject.Wrap((object)list);
        Assert.True(WrappedO is (NSArray));
        objArray = (object[])WrappedO.ToObject();
        Assert.True(objArray.Length == array.Length);

        WrappedO = NSObject.Wrap((object)map);
        Assert.True(WrappedO is (NSDictionary));
        var dict = (NSDictionary)WrappedO;
        Assert.True(((NSNumber)dict.ObjectForKey("int")).ToLong()  == i);
        Assert.True(((NSNumber)dict.ObjectForKey("long")).ToLong() == lng);
        Assert.True(((NSDate)dict.ObjectForKey("date")).Date.Equals(date));

        WrappedO = NSObject.Wrap(listOfMaps);
        Assert.True(WrappedO is (NSArray));
        var arrayOfMaps = (NSArray)WrappedO;
        Assert.True(arrayOfMaps.Count == 1);
        var firstMap = (NSDictionary)arrayOfMaps[0];
        Assert.True(((NSNumber)firstMap.ObjectForKey("int")).ToLong()  == i);
        Assert.True(((NSNumber)firstMap.ObjectForKey("long")).ToLong() == lng);
        Assert.True(((NSDate)firstMap.ObjectForKey("date")).Date.Equals(date));

        // TODO
        /*
    Object unWrappedO = WrappedO.ToObject();
    Map map2 = (Map)unWrappedO;
    Assert.True(((int)map.get("int")) == i);
    Assert.True(((long)map.get("long")) == lng);
    Assert.True(((DateTime)map.get("date")).Equals(date));*/
    }

    /**
 * Test the xml reader/writer
 */
    [Fact]
    public static void TestXml()
    {
        // Parse an example plist file
        NSObject x = PropertyListParser.Parse(new FileInfo("test-files/test1.plist"));

        // check the data in it
        var d = (NSDictionary)x;
        Assert.True(d.Count == 5);
        Assert.Equal("valueA",  ((NSString)d.ObjectForKey("keyA")).ToString());
        Assert.Equal("value&B", ((NSString)d.ObjectForKey("key&B")).ToString());

        Assert.True(((NSDate)d.ObjectForKey("date")).Date.Equals(new DateTime(2011,
                                                                              11,
                                                                              28,
                                                                              10,
                                                                              21,
                                                                              30,
                                                                              DateTimeKind.Utc)) ||
                    ((NSDate)d.ObjectForKey("date")).Date.Equals(new DateTime(2011,
                                                                              11,
                                                                              28,
                                                                              9,
                                                                              21,
                                                                              30,
                                                                              DateTimeKind.Utc)));

        Assert.True(ArrayEquals(((NSData)d.ObjectForKey("data")).Bytes,
                                [
                                    0x00, 0x00, 0x00, 0x04, 0x10, 0x41, 0x08, 0x20, 0x82
                                ]));

        var a = (NSArray)d.ObjectForKey("array");
        Assert.True(a.Count == 4);
        Assert.True(a[0].Equals(new NSNumber(true)));
        Assert.True(a[1].Equals(new NSNumber(false)));
        Assert.True(a[2].Equals(new NSNumber(87)));
        Assert.True(a[3].Equals(new NSNumber(3.14159)));

        // read/write it, make sure we get the same thing
        PropertyListParser.SaveAsXml(x, new FileInfo("test-files/out-testXml.plist"));
        NSObject y = PropertyListParser.Parse(new FileInfo("test-files/out-testXml.plist"));
        Assert.True(x.Equals(y));
    }
}