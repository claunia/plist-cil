using Claunia.PropertyList;
using System;
using System.Collections.Generic;
using Xunit;

namespace plistcil.test
{
    public class NSNumberTests
    {
        public static IEnumerable<object[]> SpanConstructorTestData()
        {
            return new List<object[]>
            {
                // INTEGER values
                // 0
                new object[] { new byte[] { 0x00 }, NSNumber.INTEGER, false, 0, 0.0 },

                // 1-byte value < sbyte.maxValue
                new object[] { new byte[] { 0x10 }, NSNumber.INTEGER, true, 16, 16.0 },

                // 1-byte value > sbyte.MaxValue
                new object[] { new byte[] { 0xFF }, NSNumber.INTEGER, true, byte.MaxValue, (double)byte.MaxValue},

                // 2-byte value < short.maxValue
                new object[] { new byte[] { 0x10, 0x00 }, NSNumber.INTEGER, true, 4096, 4096.0 },

                // 2-byte value > short.maxValue
                new object[] { new byte[] { 0xFF, 0xFF }, NSNumber.INTEGER, true, ushort.MaxValue, (double)ushort.MaxValue},

                // 4-byte value < int.maxValue
                new object[] { new byte[] { 0x10, 0x00, 0x00, 0x00 }, NSNumber.INTEGER, true, 0x10000000, 1.0 * 0x10000000 },

                // 4-bit value > int.MaxValue
                new object[] { new byte[] { 0xFF, 0xFF, 0xFF, 0xFF }, NSNumber.INTEGER, true, uint.MaxValue, (double)uint.MaxValue },

                // 64-bit value < long.MaxValue
                new object[] { new byte[] { 0x10, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00 }, NSNumber.INTEGER, true, 0x1000000000000000, 1.0 * 0x1000000000000000 },

                // 64-bit value > long.MaxValue
                new object[] { new byte[] { 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF }, NSNumber.INTEGER, true, -1, -1.0 },

                // 128-bit positive value
                new object[] { new byte[] { 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0xa0, 0x00 }, NSNumber.INTEGER, true, unchecked((long)0xffffffffffffa000), 1.0 * unchecked((long)0xffffffffffffa000) },

                // 128-bit negative value
                new object[] { new byte[] { 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff }, NSNumber.INTEGER, true, -1, -1.0 },

                // REAL values
                // 4-byte value (float)
                new object[] { new byte[] { 0x00, 0x00, 0x00, 0x00 }, NSNumber.REAL, false, 0, 0.0  },

                new object[] { new byte[] { 0x41, 0x20, 0x00, 0x00 }, NSNumber.REAL, true, 10, 10.0  },

                new object[] { new byte[] { 0x3d, 0xcc, 0xcc, 0xcd }, NSNumber.REAL, false, 0, 0.1  },

                // 8-byte value (double)
                new object[] { new byte[] { 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00 }, NSNumber.REAL, false, 0, 0.0  },

                new object[] { new byte[] { 0x40, 0x24, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00 }, NSNumber.REAL, true, 10, 10.0  },

                new object[] { new byte[] { 0x3f, 0xb9, 0x99, 0x99, 0x99, 0x99, 0x99, 0x9a }, NSNumber.REAL, false, 0, 0.1  }
            };
        }

        [Theory]
        [MemberData(nameof(SpanConstructorTestData))]
        public void SpanConstructorTest(byte[] data, int type, bool boolValue, long longValue, double doubleValue)
        {
            NSNumber number = new NSNumber((Span<byte>)data, type);
            Assert.Equal(boolValue, number.ToBool());
            Assert.Equal(longValue, number.ToLong());
            Assert.Equal(doubleValue, number.ToDouble(), 5);
        }

        [Fact]
        public void SpanConstructorInvalidValuesTest()
        {
            Assert.Throws<ArgumentNullException>(() => new NSNumber((Span<byte>)null, NSNumber.INTEGER));
            Assert.Throws<ArgumentNullException>(() => new NSNumber((Span<byte>)null, NSNumber.REAL));
            Assert.Throws<ArgumentOutOfRangeException>(() => new NSNumber((Span<byte>)Array.Empty<byte>(), NSNumber.INTEGER));
            Assert.Throws<ArgumentException>(() => new NSNumber((Span<byte>)Array.Empty<byte>(), NSNumber.REAL));
            Assert.Throws<ArgumentException>(() => new NSNumber((Span<byte>)Array.Empty<byte>(), 9));
        }

        [Fact]
        public void StringAndTypeConstructorInvalidValuesTest()
        {
            Assert.Throws<ArgumentNullException>(() => new NSNumber((string)null, NSNumber.INTEGER));
            Assert.Throws<ArgumentNullException>(() => new NSNumber((string)null, NSNumber.REAL));
            Assert.Throws<ArgumentException>(() => new NSNumber("0", 9));
        }

        [Fact]
        public static void NSNumberConstructorTest()
        {
            NSNumber number = new NSNumber("10032936613", NSNumber.INTEGER);
            Assert.Equal(NSNumber.INTEGER, number.GetNSNumberType());
            Assert.Equal(10032936613,      number.ToObject());
        }

        [Fact]
        public static void NSNumberWithDecimalTest()
        {
            NSNumber number = new NSNumber("1360155352.748765", NSNumber.REAL);
            Assert.Equal("1360155352.748765", number.ToString());
        }

        // The tests below make sure the numbers are being parsed correctly, and do not depend on the culture info
        // being set. Especially, decimal point may vary between cultures and we don't want to take a dependency on that
        // The value being used comes seen in a real property list:
        // <key>TimeZoneOffsetFromUTC</key>
        // <real>7200.000000</real>

        #if !NETCORE
        [Fact]
        [UseCulture("en-US")]
        public static void ParseNumberEnTest()
        {
            NSNumber number = new NSNumber("7200.000001");
            Assert.True(number.isReal());
            Assert.Equal(7200.000001d, number.ToDouble());
        }

        [Fact]
        [UseCulture("nl-BE")]
        public static void ParseNumberNlTest()
        {
            // As seen in a real property list:
            // <key>TimeZoneOffsetFromUTC</key>
            // <real>7200.000000</real>
            NSNumber number = new NSNumber("7200.000001");
            Assert.True(number.isReal());
            Assert.Equal(7200.000001d, number.ToDouble());
        }

        [Fact]
        [UseCulture("en-US")]
        public static void ParseNumberEnTest2()
        {
            // As seen in a real property list:
            // <key>TimeZoneOffsetFromUTC</key>
            // <real>7200.000000</real>
            NSNumber number = new NSNumber("7200.000000", NSNumber.REAL);
            Assert.True(number.isReal());
            Assert.Equal(7200d, number.ToDouble());
        }

        [Fact]
        [UseCulture("nl-BE")]
        public static void ParseNumberNlTest2()
        {
            // As seen in a real property list:
            // <key>TimeZoneOffsetFromUTC</key>
            // <real>7200.000000</real>
            NSNumber number = new NSNumber("7200.000000", NSNumber.REAL);
            Assert.True(number.isReal());
            Assert.Equal(7200d, number.ToDouble());
        }
#endif

        public static IEnumerable<object[]> StringConstructorTestData()
        {
            return new List<object[]>
            {
                // Long values, formatted as hexadecimal values
                new object[] { "0x00", false, 0, 0.0 },
                new object[] { "0x1000", true, 0x1000, 1.0 * 0x1000 },
                new object[] { "0x00001000", true, 0x1000, 1.0 * 0x1000 },
                new object[] { "0x0000000000001000", true, 0x1000, 1.0 * 0x1000 },

                // Long values, formatted as decimal values
                new object[] { "0", false, 0, 0.0 },
                new object[] { "10", true, 10, 10.0 },
                
                // Decimal values
                new object[] { "0.0", false, 0, 0.0 },
                new object[] { "0.10", false, 0, 0.1 },
                new object[] { "3.14", true, 3, 3.14 },

                // Boolean values
                new object[] { "yes", true, 1, 1},
                new object[] { "true", true, 1, 1},
                new object[] { "Yes", true, 1, 1},
                new object[] { "True", true, 1, 1},
                new object[] { "YES", true, 1, 1},
                new object[] { "TRUE", true, 1, 1},

                new object[] { "no", false, 0, 0},
                new object[] { "false", false, 0, 0},
                new object[] { "No", false, 0, 0},
                new object[] { "False", false, 0, 0},
                new object[] { "NO", false, 0, 0},
                new object[] { "FALSE", false, 0, 0},
            };
        }

        [Theory]
        [MemberData(nameof(StringConstructorTestData))]
        public void StringConstructorTest(string value, bool boolValue, long longValue, double doubleValue)
        {
            NSNumber number = new NSNumber(value);
            Assert.Equal(boolValue, number.ToBool());
            Assert.Equal(longValue, number.ToLong());
            Assert.Equal(doubleValue, number.ToDouble(), 5);
        }

        [Fact]
        public void StringConstructorInvalidValuesTest()
        {
            Assert.Throws<ArgumentException>(() => new NSNumber(null));
            Assert.Throws<ArgumentException>(() => new NSNumber("plist"));
        }

        public static IEnumerable<object[]> Int32ConstructorTestData()
        {
            return new List<object[]>
            {
                // Long values, formatted as hexadecimal values
                new object[] { 0, false, 0, 0.0 },
                new object[] { 1, true, 1, 1.0 },
                new object[] { -1, true, -1, -1.0 },
                new object[] { int.MaxValue, true, int.MaxValue, int.MaxValue },
                new object[] { int.MinValue, true, int.MinValue, int.MinValue },
            };
        }

        [Theory]
        [MemberData(nameof(Int32ConstructorTestData))]
        public void Int32ConstructorTest(int value, bool boolValue, long longValue, double doubleValue)
        {
            NSNumber number = new NSNumber(value);
            Assert.Equal(boolValue, number.ToBool());
            Assert.Equal(longValue, number.ToLong());
            Assert.Equal(doubleValue, number.ToDouble(), 5);
        }

        public static IEnumerable<object[]> Int64ConstructorTestData()
        {
            return new List<object[]>
            {
                // Long values, formatted as hexadecimal values
                new object[] { 0, false, 0, 0.0 },
                new object[] { 1, true, 1, 1.0 },
                new object[] { -1, true, -1, -1.0 },
                new object[] { long.MaxValue, true, long.MaxValue, long.MaxValue },
                new object[] { long.MinValue, true, long.MinValue, long.MinValue },
            };
        }

        [Theory]
        [MemberData(nameof(Int64ConstructorTestData))]
        public void Int64ConstructorTest(long value, bool boolValue, long longValue, double doubleValue)
        {
            NSNumber number = new NSNumber(value);
            Assert.Equal(boolValue, number.ToBool());
            Assert.Equal(longValue, number.ToLong());
            Assert.Equal(doubleValue, number.ToDouble(), 5);
        }

        public static IEnumerable<object[]> DoubleConstructorTestData()
        {
            return new List<object[]>
            {
                // Long values, formatted as hexadecimal values
                new object[] { 0.0, false, 0, 0.0 },
                new object[] { 1.0, true, 1, 1.0 },
                new object[] { -1.0, true, -1, -1.0 },
                new object[] { double.Epsilon, false, 0, double.Epsilon },
                new object[] { double.MaxValue, true, long.MinValue /* Overflow! */, double.MaxValue },
                new object[] { double.MinValue, true, long.MinValue, double.MinValue },
            };
        }

        [Theory]
        [MemberData(nameof(DoubleConstructorTestData))]
        public void DoubleConstructorTest(double value, bool boolValue, long longValue, double doubleValue)
        {
            NSNumber number = new NSNumber(value);
            Assert.Equal(boolValue, number.ToBool());
            Assert.Equal(longValue, number.ToLong());
            Assert.Equal(doubleValue, number.ToDouble(), 5);
        }
        public static IEnumerable<object[]> BoolConstructorTestData()
        {
            return new List<object[]>
            {
                // Long values, formatted as hexadecimal values
                new object[] { false, false, 0, 0.0 },
                new object[] { true, true, 1, 1.0 },
            };
        }

        [Theory]
        [MemberData(nameof(BoolConstructorTestData))]
        public void BoolConstructorTest(bool value, bool boolValue, long longValue, double doubleValue)
        {
            NSNumber number = new NSNumber(value);
            Assert.Equal(boolValue, number.ToBool());
            Assert.Equal(longValue, number.ToLong());
            Assert.Equal(doubleValue, number.ToDouble(), 5);
        }

        [Fact]
        public void EqualTest()
        {
            NSNumber a = new NSNumber(2);
            NSNumber b = new NSNumber(2);

            Assert.Equal(a.GetHashCode(), b.GetHashCode());
            Assert.True(a.Equals(b));
            Assert.True(b.Equals(a));
        }
    }
}