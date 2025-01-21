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
using System.Linq;
using Claunia.PropertyList;
using Xunit;

namespace plistcil.test
{

    public static class ValuePreprocessorTests
    {
        [Fact]
        public static void TestPassiveDefaultPreprocessorsRegistered()
        {
            Assert.Equal(ValuePreprocessor.Preprocess(true, ValuePreprocessor.Types.BOOL), true);
            Assert.Equal(ValuePreprocessor.Preprocess(false, ValuePreprocessor.Types.BOOL), false);
            Assert.Equal(ValuePreprocessor.Preprocess("true", ValuePreprocessor.Types.BOOL), "true");
            Assert.Equal(ValuePreprocessor.Preprocess("42",ValuePreprocessor.Types.INTEGER), "42");
            Assert.Equal(ValuePreprocessor.Preprocess("3.14159",ValuePreprocessor.Types.FLOATING_POINT), "3.14159");
            Assert.Equal(ValuePreprocessor.Preprocess("2.71828", ValuePreprocessor.Types.UNDEFINED_NUMBER), "2.71828");
            Assert.Equal(ValuePreprocessor.Preprocess("TestString",ValuePreprocessor.Types.STRING), "TestString");
            Assert.Equal(ValuePreprocessor.Preprocess("TestData",ValuePreprocessor.Types.DATA), "TestData");
            byte[] value = { 0x1, 0x2, 0x4, 0x8 };
            Assert.Equal(ValuePreprocessor.Preprocess(value,ValuePreprocessor.Types.DATA), value);
            Assert.Equal(ValuePreprocessor.Preprocess("01.02.1903",ValuePreprocessor.Types.DATE), "01.02.1903");
            Assert.Equal(ValuePreprocessor.Preprocess(23.0, ValuePreprocessor.Types.DATE), 23.0);
        }

        [Fact]
        public static void TestRegisterPreprocessor()
        {
            Func<string, string> examplePreprocessor = value => new string(value.Reverse().ToArray());
            string testString = "TestString";
            string expected = "gnirtStseT";

            ValuePreprocessor.Register(examplePreprocessor, ValuePreprocessor.Types.STRING);
            string actual = ValuePreprocessor.Preprocess(testString, ValuePreprocessor.Types.STRING);

            Assert.Equal(actual, expected);

            ValuePreprocessor.Unregister<string>(ValuePreprocessor.Types.STRING);
        }

        [Fact]
        public static void TestRegisteredPreprocessorSelection()
        {
            Func<string, string> examplePreprocessor = value => new string(value.Reverse().ToArray());
            string testString = "TestString";

            ValuePreprocessor.Register(examplePreprocessor, ValuePreprocessor.Types.STRING);
            string actual = ValuePreprocessor.Preprocess(testString, ValuePreprocessor.Types.DATA);

            // assert unchanged, since the selected preprocessor != registered preprocessor
            Assert.Equal(actual, testString);

            ValuePreprocessor.Unregister<string>(ValuePreprocessor.Types.STRING);
        }

        [Fact]
        public static void TestUnregisterdPreprocessorThrows()
        {
            byte[] testArray  = { 0x1, 0x2, 0x4, 0x8 };

            // there's no registered preprocessor for byte array arguments for STRING
            Assert.Throws<ArgumentException>(() => ValuePreprocessor.Preprocess(testArray, ValuePreprocessor.Types.STRING));
        }
    }
}
