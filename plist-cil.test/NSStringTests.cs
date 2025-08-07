using Claunia.PropertyList;
using Xunit;

namespace plistcil.test;

public class NSStringTests
{
    const string START_TOKEN = "<string>";
    const string END_TOKEN   = "</string>";

    [InlineData("abc", "abc")]
    [InlineData("a>b", "a&gt;b")]
    [InlineData("a<b", "a&lt;b")]
    [InlineData("a&b", "a&amp;b")]
    [Theory]
    public void Content_IsEscaped(string value, string content)
    {
        var    element = new NSString(value);
        string xml     = element.ToXmlPropertyList();

        // Strip the leading and trailing data, so we just get the string element itself
        int    start         = xml.IndexOf(START_TOKEN) + START_TOKEN.Length;
        int    end           = xml.IndexOf(END_TOKEN);
        string actualContent = xml.Substring(start, end - start);

        Assert.Equal(content, actualContent);
    }
}