namespace PhotoCli.Tests.UnitTests.Utils;

public class CommandLineArgumentsFakesUnitTests
{
	[Theory]
	[InlineData("-f parameter1 -t parameter2", new[] { "-f", "parameter1", "-t", "parameter2" })]
	public void CommandLineHelpers_SplitCommandLineArgs_ReturnsSplitArrayOfEachString(string rawArgs, string[] argsExpected)
	{
		var argsActual = CommandLineArgumentsFakes.SplitCommandLineArgs(rawArgs);
		Assert.Equal(argsExpected, argsActual);
	}
}
