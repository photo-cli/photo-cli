namespace PhotoCli.Tests.EndToEndTests;

public class BaseCopyVerbEndToEndTests : BaseEndToEndTests
{
	protected BaseCopyVerbEndToEndTests(ITestOutputHelper testOutputHelper) : base(testOutputHelper)
	{
	}

	protected async Task<(ConsoleOutputValues, List<PhotoCsv>)> ExecuteCopy(ICollection<string> args, FileInfo fileInfo)
	{
		var actualOutput = await RunMain(args);
		var actualConsoleOutput = ParseConsoleOutput(actualOutput);
		var actualPhotoCsvModels = CsvFileHelper.ReadRecords(fileInfo);
		return (actualConsoleOutput, actualPhotoCsvModels);
	}
}
