using System;
using System.Linq;
using Claunia.PropertyList;
using Xunit;

namespace plistcil.test
{
    public static class ValuePreprocessorTests
    {
        // lock tests to make sure temporarily added / replaced preprocessors don't interfere with the other tests in this suite
        private static readonly object _testLock = new();

        [Fact]
        public static void TestPassiveDefaultPreprocessorsRegistered()
        {
            byte[] testByteArray = [0x1, 0x2, 0x4, 0x8];

            Assert.Equal(true, ValuePreprocessor.Preprocess(true, ValuePreprocessor.Type.BOOL));
            Assert.Equal(false, ValuePreprocessor.Preprocess(false, ValuePreprocessor.Type.BOOL));
            Assert.Equal("true", ValuePreprocessor.Preprocess("true", ValuePreprocessor.Type.BOOL));

            Assert.Equal("42", ValuePreprocessor.Preprocess("42", ValuePreprocessor.Type.INTEGER));
            Assert.Equal(testByteArray, ValuePreprocessor.Preprocess(testByteArray, ValuePreprocessor.Type.INTEGER));

            Assert.Equal("3.14159", ValuePreprocessor.Preprocess("3.14159", ValuePreprocessor.Type.FLOATING_POINT));
            Assert.Equal(testByteArray, ValuePreprocessor.Preprocess(testByteArray, ValuePreprocessor.Type.FLOATING_POINT));

            Assert.Equal("2.71828", ValuePreprocessor.Preprocess("2.71828", ValuePreprocessor.Type.UNDEFINED_NUMBER));

            Assert.Equal("TestString", ValuePreprocessor.Preprocess("TestString", ValuePreprocessor.Type.STRING));
            Assert.Equal(testByteArray, ValuePreprocessor.Preprocess(testByteArray, ValuePreprocessor.Type.STRING));

            Assert.Equal("TestData", ValuePreprocessor.Preprocess("TestData", ValuePreprocessor.Type.DATA));
            Assert.Equal(testByteArray, ValuePreprocessor.Preprocess(testByteArray, ValuePreprocessor.Type.DATA));

            Assert.Equal(testByteArray, ValuePreprocessor.Preprocess(testByteArray, ValuePreprocessor.Type.DATE));
            Assert.Equal("01.02.1903", ValuePreprocessor.Preprocess("01.02.1903", ValuePreprocessor.Type.DATE));
            Assert.Equal(23.0, ValuePreprocessor.Preprocess(23.0, ValuePreprocessor.Type.DATE));
        }

        [Fact]
        public static void TestRegisterPreprocessor()
        {
            lock(_testLock)
            {
                Func<string, string> examplePreprocessor = value => new string(value.Reverse().ToArray());
                string               testString          = "TestString";
                string               expected            = "gnirtStseT";

                var testType = (ValuePreprocessor.Type)42;

                ValuePreprocessor.Set(examplePreprocessor, testType);
                string actual = ValuePreprocessor.Preprocess(testString, testType);

                Assert.Equal(actual, expected);

                ValuePreprocessor.Unset<string>(testType);
            }
        }

        [Fact]
        public static void TestRegisteredPreprocessorSelection1()
        {
            lock(_testLock)
            {
                Func<short, short> examplePreprocessor = value => (short)(value - 1);
                short              testShort           = 42;
                string             testString          = "TestString";

                var testType = (ValuePreprocessor.Type)42;

                // correct value type, differing data type
                ValuePreprocessor.Set(examplePreprocessor, testType);
                ValuePreprocessor.Set(ValuePreprocessor.GetDefault<string>(), testType);

                string actual1 = ValuePreprocessor.Preprocess(testString, testType);
                short  actual2 = ValuePreprocessor.Preprocess(testShort, testType);

                // assert unchanged, since the selected preprocessor != tested preprocessor
                Assert.Equal(actual1, testString);
                Assert.NotEqual(actual2, testShort);

                ValuePreprocessor.Remove<short>(testType);
                ValuePreprocessor.Remove<string>(testType);
            }
        }

        [Fact]
        public static void TestRegisteredPreprocessorSelection2()
        {
            lock(_testLock)
            {
                Func<string, string> examplePreprocessor = value => new string(value.Reverse().ToArray());
                byte[]               testByteArray       = [0x42,];
                string               testString          = "TestString";

                var testType = (ValuePreprocessor.Type)42;

                // correct value type, differing data type
                ValuePreprocessor.Set(examplePreprocessor, testType);
                ValuePreprocessor.Set(ValuePreprocessor.GetDefault<byte[]>(), testType);

                string actual1 = ValuePreprocessor.Preprocess(testString, testType);
                byte[] actual2 = ValuePreprocessor.Preprocess(testByteArray, testType);

                Assert.NotEqual(actual1, testString);

                // assert unchanged, since the selected preprocessor != tested preprocessor
                Assert.Equal(actual2, testByteArray);

                ValuePreprocessor.Unset<string>(testType);
                ValuePreprocessor.Remove<byte[]>(testType);
            }
        }

        [Fact]
        public static void TestUnregisteredPreprocessorThrows()
        {
            int[] testArray = [1, 2, 4, 8];

            // there's no registered preprocessor for byte array arguments for STRING
            Assert.Throws<ArgumentException>(() => ValuePreprocessor.Preprocess(testArray, ValuePreprocessor.Type.STRING));
        }
    }
}