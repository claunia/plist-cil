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
using System.IO;
using NUnit.Framework;
using Claunia.PropertyList;

namespace plistcil.test
{
    [TestFixture]
    public static class IssueTest
    {
        [Test]
        public static void TestIssue4()
        {
            NSDictionary d = (NSDictionary)PropertyListParser.Parse(new FileInfo("test-files/issue4.plist"));
            Assert.True(((NSString)d.ObjectForKey("Device Name")).ToString().Equals("Kid\u2019s iPhone"));
        }

        [Test]
        public static void TestIssue7()
        {
            // also a test for issue 12
            // the issue4 test has a UTF-16-BE string in its binary representation
            NSObject x = PropertyListParser.Parse(new FileInfo("test-files/issue4.plist"));
            PropertyListParser.SaveAsBinary(x, new FileInfo("test-files/out-testIssue7.plist"));
            NSObject y = PropertyListParser.Parse(new FileInfo("test-files/out-testIssue7.plist"));
            Assert.True(x.Equals(y));
        }

        [Test]
        public static void TestIssue16()
        {
            float x = ((NSNumber)PropertyListParser.Parse(new FileInfo("test-files/issue16.plist"))).floatValue();
            Assert.True(x == (float)2.71828);
        }

        [Test]
        public static void TestIssue18()
        {
            NSNumber x = new NSNumber(-999);
            PropertyListParser.SaveAsBinary(x, new FileInfo("test-files/out-testIssue18.plist"));
            NSObject y = PropertyListParser.Parse(new FileInfo("test-files/out-testIssue18.plist"));
            Assert.True(x.Equals(y));
        }

        [Test]
        public static void TestIssue21()
        {
            String x = ((NSString)PropertyListParser.Parse(new FileInfo("test-files/issue21.plist"))).ToString();
            Assert.True(x.Equals("Lot&s of &persand&s and other escapable \"\'<>€ characters"));
        }

        [Test]
        public static void TestIssue22()
        {
            NSDictionary x1 = ((NSDictionary)PropertyListParser.Parse(new FileInfo("test-files/issue22-emoji.plist")));
            NSDictionary x2 = ((NSDictionary)PropertyListParser.Parse(new FileInfo("test-files/issue22-emoji-xml.plist")));
            PropertyListParser.SaveAsBinary(x1, new FileInfo("test-files/out-testIssue22.plist"));
            NSDictionary y1 = ((NSDictionary)PropertyListParser.Parse(new FileInfo("test-files/out-testIssue22.plist")));
            PropertyListParser.SaveAsXml(x2, new FileInfo("test-files/out-testIssue22-xml.plist"));
            NSDictionary y2 = ((NSDictionary)PropertyListParser.Parse(new FileInfo("test-files/out-testIssue22-xml.plist")));
            Assert.True(x1.Equals(x2));
            Assert.True(x1.Equals(y1));
            Assert.True(x1.Equals(y2));
            Assert.True(x2.Equals(y1));
            Assert.True(x2.Equals(y2));

            String emojiString = "Test Test, \uD83D\uDE30\u2754\uD83D\uDC4D\uD83D\uDC4E\uD83D\uDD25";

            Assert.True(emojiString.Equals(x1.ObjectForKey("emojiString").ToString()));
            Assert.True(emojiString.Equals(x2.ObjectForKey("emojiString").ToString()));
            Assert.True(emojiString.Equals(y1.ObjectForKey("emojiString").ToString()));
            Assert.True(emojiString.Equals(y2.ObjectForKey("emojiString").ToString()));
        }

        [Test]
        public static void TestIssue30()
        {
            #pragma warning disable 219
            NSArray arr = (NSArray)PropertyListParser.Parse(new FileInfo("test-files/issue30.plist"));
            #pragma warning restore 219
        }

        [Test]
        public static void TestIssue33()
        {
            #pragma warning disable 219
            NSDictionary dict = (NSDictionary)PropertyListParser.Parse(new FileInfo("test-files/issue33.pbxproj"));
            #pragma warning restore 219
        }

        [Test]
        public static void TestIssue38()
        {
            NSDictionary dict = (NSDictionary)PropertyListParser.Parse(new FileInfo("test-files/issue33.pbxproj"));
            NSObject fileRef = ((NSDictionary)((NSDictionary)dict.Get("objects")).Get("65541A9C16D13B8C00A968D5")).Get("fileRef");
            Assert.True(fileRef.Equals(new NSString("65541A9B16D13B8C00A968D5")));
        }

        [Test]
        public static void TestIssue49()
        {
            NSDictionary dict = (NSDictionary)PropertyListParser.Parse(new FileInfo("test-files/issue49.plist"));
            Assert.AreEqual(0, dict.Count);
        }
    }
}

