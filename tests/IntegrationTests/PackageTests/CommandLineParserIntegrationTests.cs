using CommandLine;

namespace PhotoCli.Tests.IntegrationTests.PackageTests;

public class CommandLineParserIntegrationTests
{
	public static TheoryData<string[], CopyOptions> Data = new()
	{
		{
			CommandLineArgumentsFakes.CopyBuildCommandLineOptions("/home/user/Desktop/path1", "/home/user/Desktop/output1", NamingStyleFakes.Valid(),
				FolderProcessTypeFakes.Valid(), NumberNamingTextStyleFakes.Valid(), CopyNoPhotoTakenDateAction.PreventProcess, CopyNoCoordinateAction.PreventProcess, true),
			new CopyOptions("/home/user/Desktop/path1", NamingStyleFakes.Valid(), FolderProcessTypeFakes.Valid(),
				NumberNamingTextStyleFakes.Valid(), CopyNoPhotoTakenDateAction.PreventProcess, CopyNoCoordinateAction.PreventProcess, "/home/user/Desktop/output1", true)
		}
	};

	[Theory]
	[MemberData(nameof(Data))]
	public void CommandLineRawArgs_Should_Be_Matched_With_OptionsObject(string[] args, CopyOptions copyOptionsExpected)
	{
		var parserResults = Parser.Default.ParseArguments<CopyOptions>(args);
		parserResults.WithParsed(commandLineOptionsActual => { commandLineOptionsActual.Should().BeEquivalentTo(copyOptionsExpected); });
	}

	[Theory]
	[InlineData("-f parameter1 -t parameter2", new[] { "-f", "parameter1", "-t", "parameter2" })]
	public void CommandLineHelpers_SplitCommandLineArgs_ReturnsSplitArrayOfEachString(string rawArgs, string[] argsExpected)
	{
		var argsActual = CommandLineArgumentsFakes.SplitCommandLineArgs(rawArgs);
		Assert.Equal(argsExpected, argsActual);
	}
}
