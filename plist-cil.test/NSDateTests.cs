﻿using System;
using Claunia.PropertyList;
using Xunit;

namespace plistcil.test;

public class NSDateTests
{
    [Fact]
    public static void ConstructorTest()
    {
        var actual   = new NSDate("2000-01-01T00:00:00Z");
        var expected = new DateTime(2000, 1, 1, 0, 0, 0, DateTimeKind.Utc);
        Assert.Equal(expected, actual.Date.ToUniversalTime());
    }

    [Fact]
    public static void MakeDateStringTest()
    {
        var    date     = new DateTime(2000, 1, 1, 0, 0, 0, DateTimeKind.Utc);
        string expected = "2000-01-01T00:00:00Z";
        string actual   = NSDate.MakeDateString(date);

        Assert.Equal(expected, actual);
    }
}