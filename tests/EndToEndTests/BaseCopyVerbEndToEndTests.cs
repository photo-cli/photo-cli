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

	protected static void VerifyCsvModelsNewPathExists(IEnumerable<PhotoCsv> actualPhotoCsvModels, string outputFolder)
	{
		var fileInfos = actualPhotoCsvModels.Select(actualPhotoCsvModel => new FileInfo(Path.Combine(outputFolder, actualPhotoCsvModel.PhotoNewPath!)));
		foreach (var fileInfo in fileInfos)
			fileInfo.Exists.Should().Be(true);
	}
}
