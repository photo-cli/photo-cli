namespace PhotoCli.Tests.EndToEndTests;

public class BaseCopyVerbEndToEndTests : BaseEndToEndTests
{
	private readonly DirectoryInfo _outputDirectory = new(OutputPath);

	protected BaseCopyVerbEndToEndTests(ITestOutputHelper testOutputHelper) : base(testOutputHelper)
	{
	}

	protected async Task<(ConsoleOutputValues, List<PhotoCsv>)> ExecuteCopy(string[] args, FileInfo fileInfo)
	{
		var actualOutput = await RunMain(args);
		var actualConsoleOutput = ParseConsoleOutput(actualOutput);
		var actualPhotoCsvModels = CsvFileHelper.ReadRecords(fileInfo);
		return (actualConsoleOutput, actualPhotoCsvModels);
	}

	protected void DeleteOutputFolderIfExists()
	{
		if (_outputDirectory.Exists)
			_outputDirectory.Delete(true);
	}
}
