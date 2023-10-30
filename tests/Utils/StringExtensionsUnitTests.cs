namespace PhotoCli.Tests.UnitTests.Utils;

public class StringExtensionsUnitTests
{
	[Theory]
	[InlineData("abc", "a", "b", "bbc")]
	[InlineData("aabc", "a", "b", "babc")]
	[InlineData("abca", "a", "b", "bbca")]
	[InlineData("abc", "d", "b", "abc")]
	public void ReplaceFirst(string value, string search, string replace, string expected)
	{
		var actual = value.ReplaceFirst(search, replace);
		actual.Should().Be(expected);
	}
}
